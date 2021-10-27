using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedoCommandHandler
{
    public List<IRedoCommand> commandList = new List<IRedoCommand>();
    public int cur_state_index;

    public void AddCommand(IRedoCommand command)
    {
        // Debug.Log(commandList.Count);
        commandList.Add(command);
    }

    public void TransitionToNewGameState()
    {
        cur_state_index += 1;
    }

    public IRedoCommand LatestCommand()
    {
        if (commandList.Count > 0)
        {
            //  pre C#8.0 : var item = integerList[integerList.Count - 1];
            //  C#8.0 : 
            return commandList[commandList.Count - 1];
        }
        return null;
    }

    public void Undo()
    {
        if (commandList.Count == 0)
            return;

        if (cur_state_index > 0)
        {
            IRedoCommand tracked_game_command = commandList[cur_state_index-1];
            tracked_game_command.Undo();
            commandList.RemoveAt(commandList.Count - 1);
            cur_state_index--;
        }
    }
}
