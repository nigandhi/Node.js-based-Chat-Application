/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ChatPackage;

import ChatWindowPackage.ChatWindow;
import java.io.OutputStream;
import java.net.Socket;
import java.io.*;

/**
 *
 * @author nirav
 */
public class FileTransferClient implements Runnable{

    private ChatWindow window;
    private Socket skt;
    public FileTransferClient(ChatWindow win){
        window=win;
    }
    @Override
    public void run() {
	String message="";
        
        System.out.println("Hello from run");
	try {
            skt= new Socket("192.168.0.18", 1339);
            //skt= new Socket("10.1.70.50", 1339);
            //OutputStream out;
            while (true) 
            {
                byte[] byteArray = new byte[1024];
                InputStream in = skt.getInputStream();
                int bytes=0;
                bytes=in.read(byteArray, 0, byteArray.length);
                String filename=new String(byteArray,"UTF-8");
                System.out.println("File name : "+filename);
                filename=filename.substring(0, bytes);                
                window.getChatWindowArea().append("Receiving File: "+filename+"\n");
                filename="./download/"+filename;                
                OutputStream os = new FileOutputStream(filename);                
                int byteRead=0;
                bytes=0;
                while((byteRead = in.read(byteArray, 0, byteArray.length)) != -1 )
                {
                    bytes+=byteRead;
                    os.write(byteArray, 0, byteRead);
                    System.out.println(byteRead);
                    String temp=new String(byteArray,"UTF-8");
                    //System.out.println(temp);
                    if(temp.contains("@End@")){
                    //      System.out.println(temp);
                            bytes-=byteRead;
                            System.out.println("File Received"+filename);
                            break;
                    }
                    //else
                    //    os.write(byteArray, 0, byteRead);  
                }
                synchronized(os){
                        os.wait(10);
                }
                window.getChatWindowArea().append(""+bytes+"\n");
                window.getChatWindowArea().append("File Received."+"\n");
                os.close();
                
                System.out.println(byteRead);
                //incoming.close();
                System.out.println("File Received...total bytes:"+bytes);

            }
	}
	catch(Exception e) {
            System.out.print("Exception in Receiver\n");
            e.printStackTrace();
	}
    }
    public void sendFile(File path)
    {
        try{ 
                //String file=path;
                byte[] filename=path.getName().getBytes();
		String file_=path.getPath();//.substring(0, file.lastIndexOf('\n'));
		File myFile = new File (file_);
		byte [] mybytearray  = new byte [(int)myFile.length()];
		FileInputStream fis = new FileInputStream(myFile);
		BufferedInputStream bis;
		bis = new BufferedInputStream(fis);
		bis.read(mybytearray,0,mybytearray.length);
		String end="End";
		byte[] end_=end.getBytes();
		OutputStream out=skt.getOutputStream();
		try {
			out.write(filename,0,filename.length);
        		out.flush();
			Thread.sleep(100);
			out.write(mybytearray, 0, mybytearray.length);
			out.flush();
			Thread.sleep(100);
			out.write(end_,0,end_.length);
			out.flush();
			System.out.println("Done."+mybytearray.length);
			Thread.sleep(100);
		}
		catch(Exception e){
			System.out.println("Exception form sender...!!");
			e.printStackTrace();
		}
	bis.close();

	}
	catch(Exception e)
	{
            e.printStackTrace();
	}        
    }    
}
