using System;
using System.Net.Sockets;
using System.Text;

namespace CardsHandlerServerPart
{
    /// <summary>
    /// Класс для обработки потока от клиента.
    /// </summary>
    public class StreamProcessor : IDisposable
    {
        private readonly NetworkStream _stream;
        private readonly byte[] _buffer = new byte[1024];
        private readonly string _receivedData;

        public StreamProcessor(TcpClient client)
        {
            _stream = client.GetStream();
            int bytesRead = _stream.Read(_buffer, 0, _buffer.Length);
            _receivedData = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
        }

        ~StreamProcessor()
        {
            Dispose();
        }

        public void Dispose()
        {
            _stream.Close();
        }

        public string GetReceivedData()
        {
            //int bytesRead = _stream.Read(_buffer, 0, _buffer.Length);

            //return Encoding.UTF8.GetString(_buffer, 0, bytesRead);
            return _receivedData;
        }

        public void SendDataToUser(string json)
        {
            byte[] responseData = Encoding.UTF8.GetBytes(
            json.ToString().ToCharArray());
            _stream.Write(responseData, 0, responseData.Length);
        }
    }
}
