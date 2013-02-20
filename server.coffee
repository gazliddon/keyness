sys = require 'sys'
my_http = require 'http'

options = {
	'help' : { description: 'Show this message', boolean:true, alias: 'h' },
	'port' : { description: 'Port to listen on',  alias: 'p', default: 6502 }
}

optimist = require('optimist')
argv = optimist.usage('Usage: $0 [--help]', options).argv

if argv.help
	sys.puts optimist.help()
	process.exit(1)


# Let's get started

# Interface to redis

class KvStore
	constructor: () ->
		@redis = require 'redis'
		@client = @redis.createClient()

	set: (_key, _value) ->
		@client.set _key, _value

	get: (_key, _func) ->
		@client.get _key, _func

kvstore = new KvStore()

# Ive not programmed in this way before - starting to understand
# the upside of functional coding and lamdas hiding a lot
# of concurrency nonsense

sys.puts "Server starting on  " + argv.port

my_http.createServer (req, res )->

	respond = (_data) ->
		res.writeHeader 200, { 'Content-Type' : 'text/plain'}
		res.write _data
		res.end()

	getData = (_func) ->
		value = ''
		req.on 'data', (_chunk) ->
			value += _chunk
		.on 'end', ()->
			_func value

	if req.url.length >=2
		key = req.url.slice 1

		if req.method == 'POST'
			getData (_value) ->
				kvstore.set key, _value
				respond 'KEY SET'

		else if req.method == 'GET'
			try
				kvstore.get key, ( err, _value ) ->
					answer = if _value == null then 'NOT FOUND' else _value
					respond answer
			catch e
				respond 'EXCEPTION'

		else
			respond 'UNHANDLED METHOD'

.listen argv.port



