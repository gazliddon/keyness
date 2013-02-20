#-------------------------------------------------------------------------------

sys = require 'sys'
puts = sys.puts  # ooof
my_http = require 'http'

#-------------------------------------------------------------------------------
# Options Parsing

options =
	'help' :
		description: 'Show this message', boolean:true, alias: 'h'
	'port' :
		description: 'Port to listen on',  alias: 'p', default: 6502

optimist = require('optimist')
argv = optimist.usage('Usage: $0 [--help]', options).argv

# Print help message and quite if asked for help
if argv.help
	sys.puts optimist.help()
	process.exit(1)


#-------------------------------------------------------------------------------
# Interface to redis - shimmed so we can replace with something
# later if ness

class KvStore
	constructor: () ->
		@redis = require 'redis'
		@client = @redis.createClient()

	set: (_key, _value) ->
		@client.set _key, _value

	# get works on a async basis so you pass
	# a callback
	get: (_key, _func) ->
		@client.get _key, _func


#-------------------------------------------------------------------------------
kvstore = new KvStore()

# Ive not programmed in this way before - starting to understand
# the upside of functional coding and lamdas hiding a lot
# of concurrency nonsense

puts "Server starting on  " + argv.port

httpcodes =
        OK: 200
        BADREQUEST: 400
        UNAUTHORIZED: 401
        SERVER_ERROR: 500
        NOT_IMPLEMENTED: 501

my_http.createServer (req, res )->

	respond = (_data, _code = httpcodes.OK ) ->
		res.writeHeader _code, { 'Content-Type' : 'text/plain' }
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
		try
			if req.method == 'DELETE'
				respond 'DELETE NOT IMPLEMENTED', httpcodes.NOT_IMPLEMENTED

			# Post and put the same in this case for REST? I think that's right
			if req.method == 'POST' or req.method == 'PUT'
				getData (_value) ->
					kvstore.set key, _value
					respond 'KEY SET'

			else if req.method == 'GET'
				kvstore.get key, ( err, _value ) ->
					if _value == null
						respond 'NOT FOUND', httpcodes.BADREQUEST
					else
						respond _value

			else
				respond 'UNHANDLED METHOD', httpcodes.BADREQUEST

		catch e
			respond 'EXCEPTION', httpcodes.SERVER_ERROR

.listen argv.port
