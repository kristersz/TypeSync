"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts = require("typescript");
exports.quotemarkTransformer = function (context) { return function (rootNode) {
    function visit(node) {
        node = ts.visitEachChild(node, visit, context);
        if (node.kind === ts.SyntaxKind.StringLiteral) {
            var stringLiteral = node;
            return ts.createLiteral(stringLiteral.text);
        }
        return node;
    }
    return ts.visitNode(rootNode, visit);
}; };
//# sourceMappingURL=transformers.js.map