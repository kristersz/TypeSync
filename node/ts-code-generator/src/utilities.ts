import * as ts from 'typescript';

export class Utilities {
    static printNode = (node: ts.Node): string => {
        const sourceFile: ts.SourceFile = ts.createSourceFile('test.ts', '', ts.ScriptTarget.ES2015, true, ts.ScriptKind.TS);
        return ts.createPrinter().printNode(ts.EmitHint.Unspecified, node, sourceFile);
    }

    static concatNodes = (nodes: ts.Node[]): string => {
        let result = '';
        let prevNodeKind = null;

        for (const node of nodes) {
            if (prevNodeKind && prevNodeKind !== node.kind) {
                result += '\n';
            }

            result += Utilities.printNode(node);
            result += '\n';

            prevNodeKind = node.kind;
        }

        return result;
    }

    static replaceQuotemarks = (text: string): string => {
        return text.replace(/"/g, '\'');
    }
}
