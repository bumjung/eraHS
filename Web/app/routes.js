'use strict';

var Q = require('q');
var _ = require('underscore');

var routes = function (app, router) {
    // application =================================================================
    app.get('/', function (req, res) {
        res.render('index', {});
    });

    // routes ======================================================================

    // middleware to use for all requests
    router.use(function (req, res, next) {
        // do logging
        console.log('Something is happening.');
        next();
    });

    router.route('/game/:data')
        .post(function (req, res) {
            console.log(JSON.parse(req.params.data));
            res.json({ result : "success" });
        });

    app.use('/api', router);

};

module.exports = routes;