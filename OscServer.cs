using AdbGodotSharp;
using Godot;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public partial class OscServer : Control
{
	delegate Variant OscPartParser(byte[] packet, int cursor);
	delegate int SizeEvaluator(Variant value);

	OscValuesRead OscValues { get; set; } = new OscValuesRead();

	[Export]
	public MainLogger Logger { get; set; }

	[Signal]
	public delegate void OscValuesUpdatedEventHandler(OscValuesRead values);

	UdpServer Server { get; set; } = new UdpServer();
	UdpClient Client { get; set; } = new UdpClient(9001);
	IPEndPoint groupEP = new IPEndPoint(IPAddress.Any.Address, 9001);

	PacketPeerUdp peer;

	public override void _Ready()
	{
		GetWindow().CloseRequested += Terminate;
	}

	void Log(string message)
	{
		Logger?.LogMessage($"[OscServer] {message}");
	}


	public void StartServer()
	{
		/*Server ??= new UdpServer();

		if (Server.Listen(9001, "127.0.0.1") != Error.Ok)
		{
			Log("Could not listen on port 9001");
			StopServer();
			return;
		}*/

		SetProcess(true);
	}

	public void StopServer()
	{
		
		Client?.Close();
		Client?.Dispose();
		Client = null;
		SetProcess(false);
	}

	public void SetSettings(UserMainSettingsResource settings)
	{
		if (settings != null) { StartServer(); } else {	StopServer(); }
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			Terminate();

		}
	}

	void Terminate()
	{
		StopServer();
		GD.Print("[OscClient] Termination of UDP Client");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Client == null) return;
		/*if (!Server.IsListening())
		{
			Log("Server not listening");
			SetProcess(false);
			return;
		}
		if (Server.Poll() != Error.Ok)
		{
			Log("Polling failed");
		}

		if (peer == null)
		{
			peer = Server.TakeConnection();
			return;
		}*/

		bool dirty = false;
		
			//peer.GetPacket();

		while (Client.Available > 0)
		{
			byte[] packet = Client.Receive(ref groupEP);
			ParseOscPacket(packet);
			dirty = true;
		}
		

		if (dirty)
		{
			EmitSignal(SignalName.OscValuesUpdated, OscValues);
		}

	}

	static Variant ParseOscString(byte[] packet, int cursor)
	{
		ArraySegment<byte> arraySlice = new ArraySegment<byte>(packet);

		return System.Text.Encoding.ASCII.GetString(arraySlice.Slice(cursor).ToArray());
	}

	static int StringSizeEvaluator(Variant value)
	{
		string s = (string)value;
		return s.Length + 1;
	}

	static int AlignOn4(int number)
	{
		return ((number + 3) & (~3));
	}

	static Variant GetOscValue(
		byte[] packet,
		OscPartParser extractor,
		SizeEvaluator sizeEvaluator,
		in int cursor,
		out int newCursor)
	{

		int startIndex = AlignOn4(cursor);
		Variant value = extractor(packet, startIndex);
		newCursor = startIndex + sizeEvaluator(value);
		return value;
	}

	static string GetOscString(byte[] packet, in int cursor, out int newCursor)
	{
		return (string)GetOscValue(packet, ParseOscString, StringSizeEvaluator, cursor, out newCursor);
	}

	static Variant OscFloat(byte[] packet, int cursor)
	{
		byte[] bigEndian = new byte[4]
		{
			packet[cursor+3],
			packet[cursor+2],
			packet[cursor+1],
			packet[cursor+0]
		};
		return BitConverter.ToSingle(bigEndian, 0);
	}

	static Variant OscInt(byte[] packet, int cursor)
	{
		byte[] bigEndian = new byte[4]
		{
			packet[cursor+3],
			packet[cursor+2],
			packet[cursor+1],
			packet[cursor+0]
		};
		return BitConverter.ToInt32(bigEndian, 0);
	}

	static Variant OscTrue(byte[] _, int __)
	{
		return true;
	}

	static Variant OscFalse(byte[] _, int __)
	{
		return false;
	}

	static int OscNumberSize(Variant _)
	{
		return 4;
	}

	static int OscBoolSize(Variant _)
	{
		return 0;
	} 

	Dictionary<char, (OscPartParser, SizeEvaluator)> jumpTable =
	new Dictionary<char, (OscPartParser, SizeEvaluator)> {
		{(char)'f', ((OscPartParser)OscFloat, (SizeEvaluator)OscNumberSize)},
		{'i', (OscInt, OscNumberSize)},
		{'T', (OscTrue, OscBoolSize)},
		{'F', (OscFalse, OscBoolSize)},
		{'s', (ParseOscString, StringSizeEvaluator)}
	};



	void ParseOscPacket(byte[] packet)
	{
		if (packet.Length == 0)
		{
			return;
		}
		int cursor = 0;
		string oscPath = GetOscString(packet, cursor, out cursor);
		string type = GetOscString(packet, cursor, out cursor);

		if (type.Length < ",x".Length)
		{
			GD.Print($"[OscClient] Unknown type {type} for {oscPath}");
		}

		char oscType = type[1];
		if (!jumpTable.ContainsKey(oscType))
		{
			return;
		}
		var (parserMethod, sizeEvaluationMethod) = jumpTable[oscType];

		var value = GetOscValue(packet, parserMethod, sizeEvaluationMethod, cursor, out cursor);

		OscValues.Set(oscPath, value);
	}
}
