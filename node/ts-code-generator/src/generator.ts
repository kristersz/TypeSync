import * as ts from 'typescript';

import * as models from './models';
import { Emitter } from './emitter';
import { Utilities } from './utilities';

export class Generator {
    constructor() {
    }

    private createImport = (importModel: models.ImportModel): ts.ImportDeclaration => {
        const importSpecifiers = importModel.names.map(name => ts.createImportSpecifier(undefined, ts.createIdentifier(name)));

        const importClause = ts.createImportClause(
            undefined, // import Address from
            // ts.createNamespaceImport(ts.createIdentifier(name)) // import * as Address from
            ts.createNamedImports(importSpecifiers) // import { Address } from
        );

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

    private createHttpErrorHandler = () => {
        return ts.createMethodDeclaration([], [], undefined, 'handleError', undefined, [],
            [this.createParameter({name: 'error', type: 'any', isPrivate: false })],
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

    private createHttpCallBlock = (name: string): ts.Block => {
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
                                                ts.createPropertyAccess(ts.createThis(), 'http'), name
                                            ), [], [ts.createPropertyAccess(ts.createThis(), 'baseUrl')]),
                                        'toPromise'
                                    ), [], []),
                                'then'
                            ), [], [ts.createArrowFunction([], [],
                                [this.createParameter({name: 'response', type: 'Response', isPrivate: false})], undefined, undefined,
                                    ts.createIdentifier('response'))
                            ]),
                        'catch'
                    ), [], [ts.createPropertyAccess(ts.createThis(), 'handleError')]
                )
            )
        ], true);
    }

    private createHttpMethod = (name: string, returnType: string, httpMethod: models.HttpMethod,
        parameters: models.ParameterModel[]) : ts.MethodDeclaration => {

        const params = parameters.map(p => this.createParameter(p));

        return ts.createMethodDeclaration([], [], undefined, name, undefined, [],
            params,
            ts.createTypeReferenceNode('Promise',
            [ts.createTypeReferenceNode(returnType, [])]),
            this.createHttpCallBlock(name)
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

        // methods
        for (const method of classModel.methods) {

            // http methods
            if (method.isHttpService) {
                classElements.push(this.createHttpMethod(method.name, method.returnType, method.httpMethod, method.parameters));
            }
        }

        classElements.push(this.createHttpErrorHandler());

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

        // generate text
        const generated = Utilities.concatNodes([...importNodes, classNode]);

        return generated;
    }

    generateEnum = (enumModel: models.EnumModel): string => {
        const enumNode = this.createEnum(enumModel);
        const generated = Utilities.printNode(enumNode);

        return generated;
    }
}
