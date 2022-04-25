//Hiriart Corales Estupinan Galvez
//New application layer protocol - Server side implementation
using System.Net.Sockets;
using System.Net;
using System.Text;

class Program{
    static void Main(){
        bool error = false;

        while(!error){
            error = Listen();
        }

        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }

    private static bool Listen(){
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());//Get the local computer's address
        IPEndPoint localPoint = new IPEndPoint(ipHostInfo.AddressList[0], 420);//Create endpoint with local IP and a port

        Console.WriteLine($"Local address and port : {localPoint.ToString()}");

        //InterNetwork is IPv4, Stream and Tcp are both for TCP
        Socket server = new Socket(localPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(localPoint);//Bind socket to endpoint

        string data = null;//Client's  message
        byte[] bytes = new Byte[1024];//Data buffer
        try{
            server.Listen(5);//Param is the max number of pending connections

            Console.WriteLine("Listening...");
            Socket handler = server.Accept();//New socket for a new connection
            Console.WriteLine($"Connected to: {handler.RemoteEndPoint.ToString()}");
            data = null;

            int bytesRec = handler.Receive(bytes);
            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);//Decode message and store it

            Console.WriteLine($"Received message: {data}");

            //Handle the recieved mesage and store it
            byte[] msg = HCEGPServer(data);

            //Send a response
            handler.Send(msg);
            handler.Shutdown(SocketShutdown.Both);//Shutdown for receiving and sending
            handler.Close();

            server.Close();

            return false;//No error

        }catch (Exception e){
            Console.WriteLine($"Error: {e.ToString()}");
            return true;//Error
        }
    }

    //HCEGPServer: Hiriart Corales Estupiñan Galvez Protocol Server side
    private static byte[] HCEGPServer(string data){
        string[] content = data.Split(';');//Separe by ';'
        if (content.Length == 3){//If there are 3 strings, that means the protocol format is correct, otherwise return an error
            string text = content[0].TrimEnd().TrimStart();//Remove empty spaces from the start and end
            string target = content[1].TrimEnd().TrimStart();
            string replacement = content[2].TrimEnd().TrimStart();

            if(target.Length != 1){
                return Encoding.UTF8.GetBytes("1;target not a single character");
            }
            if(replacement.Length != 1){
                return Encoding.UTF8.GetBytes("1;replacement not a single character");
            }

            string result = text.Replace(target, replacement);

            return Encoding.UTF8.GetBytes($"0;{result}");
        }else{
            return Encoding.UTF8.GetBytes("1;format error");
        }
    }
}