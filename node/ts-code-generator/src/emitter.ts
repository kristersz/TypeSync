import * as fs from 'fs';

export class Emitter {
    emitFile = (path: string, contents: string) => {
        fs.writeFile(path, contents, function (err) {
            if (err) {
                return console.log(err);
            }

            console.log(`File emitted to ${path}`);
        });
    }
}
