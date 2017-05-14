"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts = require("typescript");
var models = require("./models");
var utilities_1 = require("./utilities");
var transformers = require("./transformers");
var Generator = (function () {
    function Generator() {
        var _this = this;
        this.createImport = function (importModel) {
            var importClause = undefined;
            if (importModel.names.length > 0) {
                var importSpecifiers = importModel.names.map(function (name) { return ts.createImportSpecifier(undefined, ts.createIdentifier(name)); });
                importClause = ts.createImportClause(undefined, // import Address from
                // ts.createNamespaceImport(ts.createIdentifier(name)) // import * as Address from
                ts.createNamedImports(importSpecifiers) // import { Address } from
                );
            }
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
        this.createRoute = function (route) {
            var root = '/';
            if (route == null) {
                return ts.createLiteral(root);
            }
            return ts.createAdd(ts.createLiteral(root), ts.createIdentifier('id'));
            // return ts.createTemplateExpression(undefined, [ts.createTemplateSpan(ts.createIdentifier('id'), null)])
        };
        this.mapHttpMethodName = function (httpMethod) {
            switch (httpMethod) {
                case models.HttpMethod.Get:
                    return 'get';
                case models.HttpMethod.Post:
                    return 'post';
                case models.HttpMethod.Put:
                    return 'put';
                case models.HttpMethod.Patch:
                    return 'patch';
                case models.HttpMethod.Delete:
                    return 'delete';
            }
        };
        this.createHttpCallBlock = function (name, httpMethod, route) {
            var params = [];
            switch (httpMethod) {
                case models.HttpMethod.Get:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), _this.createRoute(route)));
                    break;
                case models.HttpMethod.Post:
                    params.push(ts.createPropertyAccess(ts.createThis(), 'baseUrl'));
                    params.push(ts.createIdentifier('student'));
                    break;
                case models.HttpMethod.Put:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), _this.createRoute(route)));
                    params.push(ts.createIdentifier('student'));
                    break;
                case models.HttpMethod.Delete:
                    params.push(ts.createAdd(ts.createPropertyAccess(ts.createThis(), 'baseUrl'), _this.createRoute(route)));
                    break;
            }
            return ts.createBlock([
                ts.createReturn(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createCall(ts.createPropertyAccess(ts.createPropertyAccess(ts.createThis(), 'http'), _this.mapHttpMethodName(httpMethod)), [], params), 'toPromise'), [], []), 'then'), [], [ts.createArrowFunction([], [], [_this.createParameter({ name: 'response', type: 'Response', isPrivate: false })], undefined, undefined, ts.createCall(ts.createPropertyAccess(ts.createIdentifier('response'), 'json'), [], []))
                ]), 'catch'), [], [ts.createPropertyAccess(ts.createThis(), 'handleError')]))
            ], true);
        };
        this.createHttpMethod = function (httpMethodModel) {
            var params = httpMethodModel.parameters.map(function (p) { return _this.createParameter(p); });
            return ts.createMethodDeclaration([], [], undefined, httpMethodModel.name, undefined, [], params, ts.createTypeReferenceNode('Promise', [ts.createTypeReferenceNode(httpMethodModel.returnType, [])]), _this.createHttpCallBlock(httpMethodModel.name, httpMethodModel.httpMethod, httpMethodModel.route));
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
            var hasHttpMethod = false;
            // methods
            if (classModel.methods) {
                for (var _i = 0, _a = classModel.methods; _i < _a.length; _i++) {
                    var method = _a[_i];
                    // http methods
                    var httpMethod = method;
                    if (httpMethod.httpMethod >= 0) {
                        classElements.push(_this.createHttpMethod(httpMethod));
                        hasHttpMethod = true;
                    }
                }
            }
            if (hasHttpMethod) {
                classElements.push(_this.createHttpErrorHandler());
            }
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
            // transform
            var result = ts.transform(classNode, [transformers.quotemarkTransformer]);
            var transformedClassNode = result.transformed[0];
            // print text
            var printed = utilities_1.Utilities.concatNodes(importNodes.concat([transformedClassNode]));
            // format
            // const formatted = new Formatter().format(generated);
            var replaced = utilities_1.Utilities.replaceQuotemarks(printed);
            return replaced;
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