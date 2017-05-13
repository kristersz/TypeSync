import * as bodyParser from 'body-parser';
import * as cookieParser from 'cookie-parser';
import * as express from 'express';
import * as logger from 'morgan';
import * as path from 'path';
import errorHandler = require('errorhandler');
import methodOverride = require('method-override');

import { Generator } from './generator';
import { Emitter } from './emitter';
import * as models from './models';
import { SyntaxWalker } from './syntax-walker';

export class Server {

    public app: express.Application;

    public static bootstrap(): Server {
        return new Server();
    }

    constructor() {
        // create expressjs application
        this.app = express();

        // configure application
        this.config();

        // add routes
        this.routes();

        // add api
        this.api();
    }

    public api() {
        // empty for now
    }

    public config() {
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
        this.app.use(function (err: any, req: express.Request, res: express.Response, next: express.NextFunction) {
            err.status = 404;
            next(err);
        });

        // error handling
        this.app.use(errorHandler());
    }

    private routes() {
        const router: express.Router = express.Router();

        // main route
        router.post('/generate/class', (req: express.Request, res: express.Response, next: express.NextFunction) => {
            const body = req.body;

            const path = body.outputPath as string;
            const dataModel = body.dataModel as models.ClassModel;

            const source = new Generator().generateClass(dataModel);

            new Emitter().emitFile(path, source);

            res.json({ message: 'Source generated' });
        });

        router.post('/generate/enum', (req: express.Request, res: express.Response, next: express.NextFunction) => {
            const body = req.body;

            const path = body.outputPath as string;
            const dataModel = body.dataModel as models.EnumModel;

            const source = new Generator().generateEnum(dataModel);

            new Emitter().emitFile(path, source);

            res.json({ message: 'Source generated' });
        });


        // syntax walker
        router.get('/syntax', (req: express.Request, res: express.Response, next: express.NextFunction) => {
            const path = '../../../Generated/DotNetFull/models/person.model.ts';
            const syntax = new SyntaxWalker().walk(path);

            res.json({ syntax: syntax });
        });

        // use router middleware
        this.app.use(router);
    }
}
