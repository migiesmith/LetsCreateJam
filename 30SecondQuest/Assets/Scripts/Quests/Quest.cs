using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest
{

    public float questDuration = 30.0f;
    public float progress = 0.0f;

    public delegate void QuestResultCallback(QuestResults result);
	public QuestResultCallback questResult;

	public enum QuestResults
	{
		COMPELTE,
		FAIL
	}

    public Quest()
    {

    }
	
	public virtual string getQuestText()
	{
		return "No Request Text!";
	}

	public virtual int getQuestDuration()
	{
		return Mathf.RoundToInt(questDuration);
	}

    public virtual void update()
    {
        questDuration -= Time.deltaTime;
        if (questDuration <= 0.0f)
		{
			questResult(QuestResults.FAIL);
		}
		else if(progress >= 1.0f)
		{
			questResult(QuestResults.COMPELTE);
		}
    }

    public virtual void processTile(Tile tile)
    {

    }

}
