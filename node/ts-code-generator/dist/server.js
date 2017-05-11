"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var bodyParser = require("body-parser");
var cookieParser = require("cookie-parser");
var express = require("express");
var logger = require("morgan");
var path = require("path");
var errorHandler = require("errorhandler");
var methodOverride = require("method-override");
var generator_1 = require("./generator");
var emitter_1 = require("./emitter");
var syntax_walker_1 = require("./syntax-walker");
var Server = (function () {
    function Server() {
        // create expressjs application
        this.app = express();
        // configure application
        this.config();
        // add routes
        this.routes();
        // add api
        this.api();
    }
    Server.bootstrap = function () {
        return new Server();
    };
    Server.prototype.api = function () {
        // empty for now
    };
    Server.prototype.config = function () {
        // add static paths
        this.app.use(express.static(path.join(__dirname, 'public')));
        // use logger middlware
        this.app.use(logger('dev'));
        // use json form parser middlware
        this.app.use(bodyParser.json());
        // use query string parser middlware
        this.app.use(bodyParser.urlencoded({
            extended: true
        }));
        // use cookie parker middleware middlware
        this.app.use(cookieParser('SECRET_GOES_HERE'));
        // use override middlware
        this.app.use(methodOverride());
        // catch 404 and forward to error handler
        this.app.use(function (err, req, res, next) {
            err.status = 404;
            next(err);
        });
        // error handling
        this.app.use(errorHandler());
    };
    Server.prototype.routes = function () {
        var router = express.Router();
        // main route
        router.post('/generate/class', function (req, res, next) {
            var body = req.body;
            console.log('Body: %j', body);
            var path = body.outputPath;
            var dataModel = body.dataModel;
            var source = new generator_1.Generator().generateClass(dataModel);
            new emitter_1.Emitter().emitFile(path, source);
            res.json({ message: 'Source generated' });
        });
        router.post('/generate/enum', function (req, res, next) {
            var body = req.body;
            console.log('Body: %j', body);
            var path = body.outputPath;
            var dataModel = body.dataModel;
            var source = new generator_1.Generator().generateEnum(dataModel);
            new emitter_1.Emitter().emitFile(path, source);
            res.json({ message: 'Source generated' });
        });
        // syntax walker
        router.get('/syntax', function (req, res, next) {
            var path = '../../../Generated/DotNetFull/models/person.model.ts';
            var syntax = new syntax_walker_1.SyntaxWalker().walk(path);
            res.json({ syntax: syntax });
        });
        // use router middleware
        this.app.use(router);
    };
    return Server;
}());
exports.Server = Server;
//# sourceMappingURL=server.js.map