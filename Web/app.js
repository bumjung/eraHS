// set up ======================================================================
var express = require('express');
var path = require('path');
var app = express();
var router = express.Router();
var mongoose = require('mongoose');
var bodyParser = require('body-parser');
var session = require('express-session');
var methodOverride = require('method-override');

// configuration ===============================================================
// mongoose.connect('mongodb://localhost/eraHS');

app.use("/dist", express.static(__dirname + "/dist"));
app.use("/static", express.static(__dirname + "/static"));
app.use("/bower_components", express.static(__dirname + "/bower_components"));
app.use("/src", express.static(__dirname + "/src"));
app.set('views', __dirname + '/src/view');
app.set('view engine', 'ejs');
app.set('port', process.env.PORT || 4455);

app.use(bodyParser.urlencoded({ 'extended': 'true' }));            // parse application/x-www-form-urlencoded
app.use(bodyParser.json());                                     // parse application/json
app.use(bodyParser.json({ type: 'application/vnd.api+json' })); // parse application/vnd.api+json as json
app.use(methodOverride());

// routes ======================================================================
require('./app/routes')(app, router);

// listen ======================================================================
app.listen(app.get('port'));
console.log("Listening on port localhost:" + app.get('port'));