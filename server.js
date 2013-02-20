
var sys = require("sys");  
var my_http = require("http");
var redis = require("redis");

function KVStore() {
    this.client = redis.createClient();
}

KVStore.prototype.put = function ( _key, _value ) {
    this.client.set(_key, _value );
};

KVStore.prototype.get = function ( _key, _func) {
    this.client.get( _key, _func);
};

var kvstore = new KVStore();

// I've not programmed in this way before - starting to understand
// the upside of functional coding and lamdas hiding a lot
// of concurrency nonsense

my_http.createServer( function(req,res) {

    var respond = function(_data) {
        res.writeHeader(200, {"Content-Type" : "text/plain"});
        res.write(_data);
        res.end();
    };

    var getData = function(_func) {
        var value = "";
        req.on('data', function (chunk) {
            value += chunk; })
        .on('end', function () {
            _func(value); });
    };

    if (req.url.length >= 2) {
        var key = req.url.slice(1);

        if (req.method == "POST") {
            getData( function(_value) {
                kvstore.put(key, _value);
                respond("KEY SET");
            });
        } else if (req.method == "GET") {
            try {
                kvstore.get(key, function(err, _value) {
                    respond( _value === null ? "NOT FOUND" : _value.toString() );
                });
            } catch( e ) {
                respond("EXCEPTION");
            }

        } else
            respond("UNHANDLED METHOD");
    }
}).listen(6502);

sys.puts("Server Running on 6502");
