using System;
using System.IO;
using MongoDB.Bson;

namespace MongoDB.Protocol
{
    /// <summary>
    ///   Description of Message.
    /// </summary>
    internal abstract class RequestMessageBase : MessageBase, IRequestMessage
    {
        private readonly BsonWriterSettings _bsonWriterSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageBase"/> class.
        /// </summary>
        /// <param name="bsonWriterSettings">The bson writer settings.</param>
        protected RequestMessageBase(BsonWriterSettings bsonWriterSettings){
            if(bsonWriterSettings == null)
                throw new ArgumentNullException("bsonWriterSettings");

            _bsonWriterSettings = bsonWriterSettings;
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Write(Stream stream){
            var bstream = new BufferedStream(stream);
            var writer = new BinaryWriter(bstream);
            var bwriter = new BsonWriter(bstream, _bsonWriterSettings);

            var bodySize = Header.MessageLength + CalculateBodySize(bwriter);
            if(bodySize > MaximumMessageSize)
                throw new MongoException("Maximum message length exceeded");

            writer.Write(bodySize);
            writer.Write(Header.RequestId);
            writer.Write(Header.ResponseTo);
            writer.Write((int)Header.OpCode);
            writer.Flush();
            WriteBody(bwriter);
            bwriter.Flush();
			Header.MessageLength = bodySize;
        }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected abstract void WriteBody(BsonWriter writer);

        /// <summary>
        /// Calculates the size of the body.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        protected abstract int CalculateBodySize(BsonWriter writer);
    }
}