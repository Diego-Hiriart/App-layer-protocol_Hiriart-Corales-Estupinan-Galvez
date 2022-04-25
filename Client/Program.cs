//Hiriart Corales Estupinan Galvez
//New application layer protocol - Server side implementation
using System.Net.Sockets;
using System.Net;
using System.Text;

class Program{
    static void Main(){
        bool error = false;
        while(!error){
            Console.WriteLine("Enter message: ");
            string message = Console.ReadLine();
            error = Talk(message);
        }

        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }

    private static bool Talk(string message){
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());//Get local address 
        IPEndPoint remoteEP = new IPEndPoint(ipHostInfo.AddressList[0], 420);  

        Console.WriteLine($"Remote address and port : {remoteEP.ToString()}");

        //InterNetwork is IPv4, Stream and Tcp are both for TCP
        Socket client = new Socket(ipHostInfo.AddressList[0].AddressFamily, 
            SocketType.Stream, ProtocolType.Tcp);

        string response = null;//Server response
        byte[] bytes = new Byte[1024];//Data buffer
        try{
            Console.WriteLine("Attempting connection");
            client.Connect(remoteEP);//Connect to remote EP
            response = null;

            Console.WriteLine($"Socket connected to {client.RemoteEndPoint.ToString()}");

            //Encode the message to be sent
            byte[] msg = Encoding.UTF8.GetBytes(message);

            //Send the encoded data
            client.Send(msg);

            //Receive response
            int bytesRec = client.Receive(bytes);
            response += Encoding.UTF8.GetString(bytes, 0, bytesRec);//Decode response

            //Handle the response
            string result = HCEGPClient(response);
            Console.WriteLine(result);

            client.Shutdown(SocketShutdown.Both);//Shutdown for receiving and sending
            client.Close();

            return false;//No error

        }catch (Exception e){
            Console.WriteLine($"Error: {e.ToString()}");
            return true;//Error
        }
    }

    //HCEGPClient: Hiriart Corales Estupiñan Galvez Protocol Client side
    private static string HCEGPClient(string response){
        string[] content = response.Split(';');
        string resultType = content[0];//Type of result, 0 for success, 1 for error
        string result = content[1];//The result of the protocol execution, either the string with replacements or an error message
        if (Int32.Parse(resultType) == 0){
            return $"Replacements successful, result: {result}";
        }else if(Int32.Parse(resultType) == 1){
            return $"An error acurred: {result}";
        }else{
            return "Unknown response";
        }
    }
}