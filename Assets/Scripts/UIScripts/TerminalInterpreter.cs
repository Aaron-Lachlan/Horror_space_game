using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TerminalInterpreter : MonoBehaviour
{
    [SerializeField] Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"Black", "#000000"},
        {"Red", "#ff0000"},
        {"Green", "#00ff00"},
        {"Blue", "#0000ff"}
    };

    List<String> response = new List<string>();
    public List<String> Interpret(string userInput)
    {
        response.Clear();

        string[] arguments = userInput.Split();

        if(arguments[0] == "help")
        {
            //Give player some info
            response.Add("launch: starts program");
            response.Add("settings: change program settings");
            response.Add("quit: quit program");
            return response;
        }
        if(arguments[0] == "launch")
        {
            //Give player some info
            response.Add("launching program...");
            
            return response;
        }
        if(arguments[0] == "settings")
        {
            //Give player some info
            response.Add("opening program settings...");
            
            return response;
        }
        if(arguments[0] == "quit")
        {
            //Give player some info
            response.Add("closing program...");
            
            return response;
        }
        if(arguments[0] == "bushwizards")
        {
            //Give player some info
            response.Add("Dave's Place");
            
            return response;
        }
        else
        {
            response.Add("Input not recognized. Type \"help\" for a list of comands.");

            return response;
        }
    }
}
