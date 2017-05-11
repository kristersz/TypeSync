"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var fs_1 = require("fs");
var ts = require("typescript");
var SyntaxWalker = (function () {
    function SyntaxWalker() {
        var _this = this;
        this._result = '';
        this.walk = function (path) {
            var sourceFile = ts.createSourceFile('test.ts', fs_1.readFileSync(path).toString(), ts.ScriptTarget.ES2015, true);
            _this.showSyntax(sourceFile);
            return _this._result;
        };
        this.showSyntax = function (sourceFile) {
            _this.showNodeSyntax(sourceFile);
        };
        this.showNodeSyntax = function (node) {
            var text = node.kind
                + ' : '
                + ts.SyntaxKind[node.kind]
                + ' : ' + node.getText();
            console.log(text);
            _this._result += text;
            ts.forEachChild(node, _this.showNodeSyntax);
        };
    }
    return SyntaxWalker;
}());
exports.SyntaxWalker = SyntaxWalker;
//# sourceMappingURL=syntax-walker.js.map