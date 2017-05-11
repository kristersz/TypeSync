import { readFileSync } from 'fs';
import * as ts from 'typescript';

export class SyntaxWalker {
    private _result = '';

    walk = (path: string): string => {
        const sourceFile = ts.createSourceFile('test.ts', readFileSync(path).toString(), ts.ScriptTarget.ES2015, true);

        this.showSyntax(sourceFile);

        return this._result;
    }

    showSyntax = (sourceFile: ts.SourceFile) => {
        this.showNodeSyntax(sourceFile);
    }

    showNodeSyntax = (node: ts.Node) => {
        const text = node.kind
            + ' : '
            + ts.SyntaxKind[node.kind]
            + ' : ' + node.getText();

        console.log(text);
        this._result += text;

        ts.forEachChild(node, this.showNodeSyntax);
    }
}
