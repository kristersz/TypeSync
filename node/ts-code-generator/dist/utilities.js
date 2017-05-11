"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts = require("typescript");
var Utilities = (function () {
    function Utilities() {
    }
    return Utilities;
}());
Utilities.printNode = function (node) {
    var sourceFile = ts.createSourceFile('test.ts', '', ts.ScriptTarget.ES2015, true, ts.ScriptKind.TS);
    return ts.createPrinter().printNode(ts.EmitHint.Unspecified, node, sourceFile);
};
Utilities.concatNodes = function (nodes) {
    var result = '';
    var prevNodeKind = null;
    for (var _i = 0, nodes_1 = nodes; _i < nodes_1.length; _i++) {
        var node = nodes_1[_i];
        if (prevNodeKind && prevNodeKind !== node.kind) {
            result += '\n';
        }
        result += Utilities.printNode(node);
        result += '\n';
        prevNodeKind = node.kind;
    }
    return result;
};
exports.Utilities = Utilities;
//# sourceMappingURL=utilities.js.map