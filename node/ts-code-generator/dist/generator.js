"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts = require("typescript");
var models = require("./models");
var utilities_1 = require("./utilities");
var Generator = (function () {
    function Generator() {
        var _this = this;
        this.createImport = function (importModel) {
            var importSpecifiers = importModel.names.map(function (name) { return ts.createImportSpecifier(undefined, ts.createIdentifier(name)); });
            var importClause = ts.createImportClause(undefined, // import Address from
            // ts.createNamespaceImport(ts.createIdentifier(name)) // import * as Address from
            ts.createNamedImports(importSpecifiers) // import { Address } from
            );
            return ts.createImportDeclaration([], [], importClause, ts.createLiteral(importModel.path));
        };
        this.createProperty = function (propertyModel) {
            var modifiers = [];
            if (propertyModel.isPrivate) {
                modifiers.push(ts.createToken(ts.SyntaxKind.PrivateKeyword));
            }
            var propertyType = undefined;
            if (propertyModel.type) {
                propertyType = ts.createTypeReferenceNode(propertyModel.type, []);
            }
            var initializer = undefined;
            if (propertyModel.initialValue) {
                initializer = ts.createLiteral(propertyModel.initialValue);
            }
            return ts.createProperty([], modifiers, propertyModel.name, undefined, propertyType, initializer);
        };
        this.createParameter = function (parameterModel) {
            var modifiers = [];
            if (parameterModel.isPrivate) {
                modifiers.push(ts.createToken(ts.SyntaxKind.PrivateKeyword));
            }
            return ts.createParameter([], modifiers, undefined, parameterModel.name, undefined, ts.createTypeReferenceNode(parameterModel.type, []), undefined);
        };
        this.createHttpErrorHandler = function () {
            return ts.createMethodDeclaration([], [], undefined, 'handleError', undefined, [], [_this.createParameter({ name: 'error', type: 'any', isPrivate: false })], ts.createTypeReferenceNode('Promise', [ts.createTypeReferenceNode('any', [])]), ts.createBlock([
                ts.createReturn(ts.createCall(ts.createPropertyAccess(ts.createIdentifier('Promise'), 'reject'), [], [ts.createLogicalOr(ts.createPropertyAccess(ts.createIdentifier('error'), 'message'), ts.createIdentifier('error'))]))
            ], true));
        };
        this.createHttpCallBlock = function (name, httpMethod) {
            var params = [];
            switch (httpMethod) {
                case models.HttpMethod.Get:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), ts.createAdd(ts.createLiteral('/'), ts.createIdentifier('id'))));
                    break;
                case models.HttpMethod.Post:
                    params.push(ts.createPropertyAccess(ts.createThis(), 'baseUrl'));
                    params.push(ts.createIdentifier('student'));
                    break;
                case models.HttpMethod.Put:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), ts.createAdd(ts.createLiteral('/'), ts.createIdentifier('id'))));
                    params.push(ts.createIdentifier('student'));
                    break;
                case models.HttpMethod.Delete:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), ts.createAdd(ts.createLiteral('/'), ts.createIdentifier('id'))));
                    break;
            }
            return ts.createBlock([
                ts.createReturn(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createPropertyAccess(ts.createThis(), 'http'), name), [], params), 'toPromise'), [], []), 'then'), [], [ts.createArrowFunction([], [], [_this.createParameter({ name: 'response', type: 'Response', isPrivate: false })], undefined, undefined, ts.createIdentifier('response'))
                ]), 'catch'), [], [ts.createPropertyAccess(ts.createThis(), 'handleError')]))
            ], true);
        };
        this.createHttpMethod = function (name, returnType, httpMethod, parameters) {
            var params = parameters.map(function (p) { return _this.createParameter(p); });
            return ts.createMethodDeclaration([], [], undefined, name, undefined, [], params, ts.createTypeReferenceNode('Promise', [ts.createTypeReferenceNode(returnType, [])]), _this.createHttpCallBlock(name, httpMethod));
        };
        this.createConstructor = function (constructorModel) {
            var parameters = constructorModel.parameters.map(function (p) { return _this.createParameter(p); });
            return ts.createConstructor([], [], parameters, ts.createBlock([]));
        };
        this.createClass = function (classModel) {
            // decorators
            var decoratorNodes = classModel.decorators.map(function (d) { return ts.createDecorator(ts.createIdentifier(d)); });
            // class modifiers
            var modifiers = [
                ts.createToken(ts.SyntaxKind.ExportKeyword)
            ];
            // type parameters
            var typeParameters = classModel.typeParameters.map(function (p) { return ts.createTypeParameterDeclaration(p, undefined, undefined); });
            // base class or interfaces
            var heritageClauses = [];
            if (classModel.baseClass) {
                heritageClauses.push(ts.createHeritageClause(ts.SyntaxKind.ExtendsKeyword, [ts.createExpressionWithTypeArguments(null, ts.createIdentifier(classModel.baseClass))]));
            }
            var classElements = [];
            // properties
            classElements.push.apply(classElements, classModel.properties.map(function (p) { return _this.createProperty(p); }));
            // constructor
            if (classModel.constructorDef) {
                classElements.push(_this.createConstructor(classModel.constructorDef));
            }
            // methods
            for (var _i = 0, _a = classModel.methods; _i < _a.length; _i++) {
                var method = _a[_i];
                // http methods
                if (method.isHttpService) {
                    classElements.push(_this.createHttpMethod(method.name, method.returnType, method.httpMethod, method.parameters));
                }
            }
            classElements.push(_this.createHttpErrorHandler());
            return ts.createClassDeclaration(decoratorNodes, modifiers, classModel.name, typeParameters, heritageClauses, classElements);
        };
        this.createEnum = function (enumModel) {
            // enum modifiers
            var modifiers = [
                ts.createToken(ts.SyntaxKind.ExportKeyword)
            ];
            var enumMembers = enumModel.members.map(function (m) { return ts.createEnumMember(m.name, ts.createNumericLiteral(m.value)); });
            return ts.createEnumDeclaration([], modifiers, enumModel.name, enumMembers);
        };
        this.generateClass = function (classModel) {
            // imports
            var importNodes = classModel.imports.map(function (model) { return _this.createImport(model); });
            // class declaration
            var classNode = _this.createClass(classModel);
            // generate text
            var generated = utilities_1.Utilities.concatNodes(importNodes.concat([classNode]));
            return generated;
        };
        this.generateEnum = function (enumModel) {
            var enumNode = _this.createEnum(enumModel);
            var generated = utilities_1.Utilities.printNode(enumNode);
            return generated;
        };
    }
    return Generator;
}());
exports.Generator = Generator;
//# sourceMappingURL=generator.js.map