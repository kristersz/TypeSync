"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var fs = require("fs");
var Emitter = (function () {
    function Emitter() {
        this.emitFile = function (path, contents) {
            fs.writeFile(path, contents, function (err) {
                if (err) {
                    return console.log(err);
                }
                console.log("File emitted to " + path);
            });
        };
    }
    return Emitter;
}());
exports.Emitter = Emitter;
//# sourceMappingURL=emitter.js.map