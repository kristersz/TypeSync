import * as ts from 'typescript';

export const quotemarkTransformer = <T extends ts.Node>(context: ts.TransformationContext) => (rootNode: T) => {
    function visit(node: ts.Node): ts.Node {
        node = ts.visitEachChild(node, visit, context);

        if (node.kind === ts.SyntaxKind.StringLiteral) {
            const stringLiteral = node as ts.StringLiteral;

            return ts.createLiteral(stringLiteral.text);
        }

        return node;
    }

    return ts.visitNode(rootNode, visit);
};
