using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerCommandCallback {

    void OnCommandStart(string commandName, PlayerCommand command);
    void OnCommandEnd(string commandName, PlayerCommand command);
	
}
