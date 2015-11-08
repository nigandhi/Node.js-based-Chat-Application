/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ChatPackage;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.Socket;
import ChatWindowPackage.ChatWindow;
import java.io.BufferedWriter;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
/**
 *
 * @author nirav
 */
public class ChatClient implements Runnable{

    private ChatWindow window;
    private Socket skt;
    public ChatClient(ChatWindow win){
        window=win;
    }
    @Override
    public void run() {
	String message="";
        System.out.println("Hello from run");
	try {
            skt= new Socket("192.168.0.18", 1337);
            //skt= new Socket("10.1.70.50", 1337);  
            while (true) 
            {				
		BufferedReader in = new BufferedReader(new InputStreamReader(skt.getInputStream()));
		//System.out.print("Received string: '");
		//while (!in.ready()) {}
		message=in.readLine();                
                window.getChatWindowArea().append("Windows: "+message+"\n");
                System.out.println(message);
            }
	}
	catch(Exception e) {
            System.out.print("Exception in Receiver\n");
            e.printStackTrace();
	}
    }
    public void sendMessage()
    {
        try{ 
            OutputStream os = skt.getOutputStream();
            OutputStreamWriter osw = new OutputStreamWriter(os);
            BufferedWriter bw = new BufferedWriter(osw);		 
            String data = window.getChatTextArea().getText() ;		 
            String sendMessage = data;
            bw.write(sendMessage);
            bw.flush();
            window.getChatWindowArea().append("Linux: "+sendMessage+"\n");
            window.getChatTextArea().setText("");
	}
	catch(Exception e)
	{
            System.out.print("Exception in Sender\n");
            e.printStackTrace();
	}
        
    }
    
}
