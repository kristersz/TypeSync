import * as ts from 'typescript';

import * as models from './models';
import { Emitter } from './emitter';
import { Formatter } from './formatter';
import { Utilities } from './utilities';
import * as transformers from './transformers';

export class Generator {
    constructor() {
    }

    private createImport = (importModel: models.ImportModel): ts.ImportDeclaration => {
        let importClause = undefined;

        if (importModel.names.length > 0) {
            const importSpecifiers = importModel.names.map(name => ts.createImportSpecifier(undefined, ts.createIdentifier(name)));

            importClause = ts.createImportClause(
                undefined, // import Address from
                // ts.createNamespaceImport(ts.createIdentifier(name)) // import * as Address from
                ts.createNamedImports(importSpecifiers) // import { Address } from
            );
        }

        return ts.createImportDeclaration([], [], importClause, ts.createLiteral(importModel.path));
    }

    private createProperty = (propertyModel: models.PropertyModel): ts.PropertyDeclaration => {
        const modifiers = [];

        if (propertyModel.isPrivate) {
            modifiers.push(ts.createToken(ts.SyntaxKind.PrivateKeyword));
        }

        let propertyType = undefined;

        if (propertyModel.type) {
            propertyType = ts.createTypeReferenceNode(propertyModel.type, []);
        }

        let initializer = undefined;

        if (propertyModel.initialValue) {
            initializer = ts.createLiteral(propertyModel.initialValue);
        }

        return ts.createProperty([], modifiers, propertyModel.name, undefined, propertyType, initializer);
    }

    private createParameter = (parameterModel: models.ParameterModel): ts.ParameterDeclaration => {
        const modifiers = [];

        if (parameterModel.isPrivate) {
            modifiers.push(ts.createToken(ts.SyntaxKind.PrivateKeyword));
        }

        return ts.createParameter([], modifiers, undefined, parameterModel.name, undefined,
            ts.createTypeReferenceNode(parameterModel.type, []), undefined);
    }

    private createHttpErrorHandler = (): ts.MethodDeclaration => {
        return ts.createMethodDeclaration([], [], undefined, 'handleError', undefined, [],
            [this.createParameter({ name: 'error', type: 'any', isPrivate: false })],
            ts.createTypeReferenceNode('Promise', [ts.createTypeReferenceNode('any', [])]),
            ts.createBlock([
                ts.createReturn(
                    ts.createCall(
                        ts.createPropertyAccess(ts.createIdentifier('Promise'), 'reject'),
                        [],
                        [ts.createLogicalOr(ts.createPropertyAccess(ts.createIdentifier('error'), 'message'), ts.createIdentifier('error'))]
                    )
                )
            ], true)
        );
    }

    private createRoute = (route: string): ts.Expression => {
        const root = '/';

        if (route == null) {
            return ts.createLiteral(root);
        }

        return ts.createAdd(ts.createLiteral(root), ts.createIdentifier('id'));

        // return ts.createTemplateExpression(undefined, [ts.createTemplateSpan(ts.createIdentifier('id'), null)])
    }

    private mapHttpMethodName = (httpMethod: models.HttpMethod) => {
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
    }

    private createHttpCallBlock = (name: string, httpMethod: models.HttpMethod, route: string): ts.Block => {
        const params = [];

        switch (httpMethod) {
            case models.HttpMethod.Get:
                params.push(ts.createAdd(
                    ts.createPropertyAccess(ts.createThis(), 'baseUrl'),
                    this.createRoute(route)
                ));
                break;
            case models.HttpMethod.Post:
                params.push(ts.createPropertyAccess(ts.createThis(), 'baseUrl'));
                params.push(ts.createIdentifier('student'));
                break;
            case models.HttpMethod.Put:
                params.push(ts.createAdd(
                    ts.createPropertyAccess(ts.createThis(), 'baseUrl'),
                    this.createRoute(route)
                ));
                params.push(ts.createIdentifier('student'));
                break;
            case models.HttpMethod.Delete:
                params.push(ts.createAdd(
                    ts.createPropertyAccess(ts.createThis(), 'baseUrl'),
                    this.createRoute(route)
                ));
                break;
        }

        return ts.createBlock([
            ts.createReturn(
                ts.createCall(
                    ts.createPropertyAccess(
                        ts.createCall(
                            ts.createPropertyAccess(
                                ts.createCall(
                                    ts.createPropertyAccess(
                                        ts.createCall(
                                            ts.createPropertyAccess(
                                                ts.createPropertyAccess(ts.createThis(), 'http'), this.mapHttpMethodName(httpMethod)
                                            ), [], params),
                                        'toPromise'
                                    ), [], []),
                                'then'
                            ), [], [ts.createArrowFunction([], [],
                                [this.createParameter({ name: 'response', type: 'Response', isPrivate: false })], undefined, undefined,
                                ts.createCall(ts.createPropertyAccess(ts.createIdentifier('response'), 'json'), [], []))
                            ]),
                        'catch'
                    ), [], [ts.createPropertyAccess(ts.createThis(), 'handleError')]
                )
            )
        ], true);
    }

    private createHttpMethod = (httpMethodModel: models.HttpMethodModel): ts.MethodDeclaration => {

        const params = httpMethodModel.parameters.map(p => this.createParameter(p));

        return ts.createMethodDeclaration([], [], undefined, httpMethodModel.name, undefined, [],
            params,
            ts.createTypeReferenceNode('Promise', [ts.createTypeReferenceNode(httpMethodModel.returnType, [])]),
            this.createHttpCallBlock(httpMethodModel.name, httpMethodModel.httpMethod, httpMethodModel.route)
        );
    }

    private createConstructor = (constructorModel: models.ConstructorModel): ts.ConstructorDeclaration => {
        const parameters = constructorModel.parameters.map(p => this.createParameter(p));

        return ts.createConstructor([], [], parameters, ts.createBlock([]));
    }

    private createClass = (classModel: models.ClassModel): ts.ClassDeclaration => {
        // decorators
        const decoratorNodes = classModel.decorators.map(d => ts.createDecorator(ts.createIdentifier(d)));

        // class modifiers
        const modifiers = [
            ts.createToken(ts.SyntaxKind.ExportKeyword)
        ];

        // type parameters
        const typeParameters = classModel.typeParameters.map(p => ts.createTypeParameterDeclaration(p, undefined, undefined));

        // base class or interfaces
        const heritageClauses = [];
        if (classModel.baseClass) {
            heritageClauses.push(ts.createHeritageClause(
                ts.SyntaxKind.ExtendsKeyword,
                [ts.createExpressionWithTypeArguments(null, ts.createIdentifier(classModel.baseClass))]
            ))
        }

        const classElements = [];

        // properties
        classElements.push(...classModel.properties.map(p => this.createProperty(p)));

        // constructor
        if (classModel.constructorDef) {
            classElements.push(this.createConstructor(classModel.constructorDef));
        }

        let hasHttpMethod = false;

        // methods
        if (classModel.methods) {
            for (const method of classModel.methods) {

                // http methods
                const httpMethod = method as models.HttpMethodModel
                if (httpMethod.httpMethod >= 0) {
                    classElements.push(this.createHttpMethod(httpMethod));
                    hasHttpMethod = true;
                }
            }
        }

        if (hasHttpMethod) {
            classElements.push(this.createHttpErrorHandler());
        }

        return ts.createClassDeclaration(decoratorNodes, modifiers, classModel.name, typeParameters, heritageClauses, classElements);
    }

    private createEnum = (enumModel: models.EnumModel): ts.EnumDeclaration => {
        // enum modifiers
        const modifiers = [
            ts.createToken(ts.SyntaxKind.ExportKeyword)
        ];

        const enumMembers = enumModel.members.map(m => ts.createEnumMember(m.name, ts.createNumericLiteral(m.value)));

        return ts.createEnumDeclaration([], modifiers, enumModel.name, enumMembers);
    }

    generateClass = (classModel: models.ClassModel): string => {
        // imports
        const importNodes = classModel.imports.map(model => this.createImport(model));

        // class declaration
        const classNode = this.createClass(classModel);

        // transform
        const result: ts.TransformationResult<ts.Node> = ts.transform(
            classNode, [transformers.quotemarkTransformer]
        );

        const transformedClassNode: ts.Node = result.transformed[0];

        // print text
        const printed = Utilities.concatNodes([...importNodes, transformedClassNode]);

        // format
        // const formatted = new Formatter().format(generated);
        const replaced = Utilities.replaceQuotemarks(printed);

        return replaced;
    }

    generateEnum = (enumModel: models.EnumModel): string => {
        const enumNode = this.createEnum(enumModel);
        const generated = Utilities.printNode(enumNode);

        return generated;
    }
}
