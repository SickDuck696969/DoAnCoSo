using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Text;

public class StockfishController : MonoBehaviour
{
    private Process stockfish;
    private StreamWriter stockfishInput;
    private StreamReader stockfishOutput;

    void Start()
    {
        StartStockfish();
        StartCoroutine(TestMove());
    }

    void StartStockfish()
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = Application.streamingAssetsPath + "/stockfish-windows-x86-64-avx2.exe";
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        stockfish = new Process();
        stockfish.StartInfo = psi;
        stockfish.Start();

        stockfishInput = stockfish.StandardInput;
        stockfishOutput = stockfish.StandardOutput;
    }

    public void SendCommand(string command)
    {
        stockfishInput.WriteLine(command);
        stockfishInput.Flush();
    }

    public string ReadResponse()
    {
        return stockfishOutput.ReadLine();
    }

    public IEnumerator GetBestMove(string fen, System.Action<string> onMoveReceived)
    {
        SendCommand("uci");
        yield return new WaitForSeconds(0.1f);

        SendCommand("position fen " + fen);
        SendCommand("go depth 15");  // Adjust depth as needed

        string bestMove = "";
        while (true)
        {
            string response = ReadResponse();
            UnityEngine.Debug.Log(response);
            if (response.StartsWith("bestmove"))
            {
                bestMove = response.Split(' ')[1];
                break;
            }
        }

        onMoveReceived?.Invoke(bestMove);
    }

    IEnumerator TestMove()
    {
        string startingFEN = "rn1qkbnr/ppp1pppp/8/3p4/8/5NP1/PPPPPPBP/RNBQK2R w KQkq - 0 4";
        yield return StartCoroutine(GetBestMove(startingFEN, move =>
        {
            UnityEngine.Debug.Log("Best Move: " + move);
        }));
    }

    private void OnApplicationQuit()
    {
        SendCommand("quit");
        stockfish.Close();
    }
}
