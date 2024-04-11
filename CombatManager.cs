using System.Runtime.CompilerServices;
using TTRPG_manager;

public static class CombatManager
{
    public static int turn = 0; // Tracks the number of complete rounds of combat
    public static bool isPlayersTurn = false;
    public static bool isEnemiesTurn = false;
    public static bool isAmbush = false;
    public static int totalTurns = 0; // Tracks the number of individual actions taken

    public static void NextTurn()
    {
        AppConfig config = ConfigManager.LoadConfig();

        // Reduce skill cooldowns for the active party
        foreach (Character character in config.Parties[config.selectedPartyIndex].Members)
        {
            foreach (Skill skill in character.Skills)
            {
                skill.Cooldown = Math.Min(skill.BaseCooldown, skill.Cooldown+1);
            }
        }

        if (!isPlayersTurn && !isEnemiesTurn) 
        {
            StartCombat();
        }
        // Manage turn progression
        if (isEnemiesTurn)
        {
            isPlayersTurn = true;
            isEnemiesTurn = false;
            if (!isAmbush)
            {
                EndRound();
            }
        }
        else if (isPlayersTurn)
        {
            isPlayersTurn = false;
            isEnemiesTurn = true;
            if (isAmbush)
            {
                EndRound();
            }

        }
        totalTurns++;
        ConfigManager.SaveConfig(config);
    }

    private static void EndRound()
    {
        // Check if the combat began with an ambush to decide who acts first in the new round
        if (isAmbush)
        {
            isEnemiesTurn = true;
            isPlayersTurn = false;
        }
        else
        {
            isPlayersTurn = true;
            isEnemiesTurn = false;
        }
        turn++; // Only increment the round counter after both have had their turn
    }

    public static void ResetTurn()
    {
        turn = 0;
        totalTurns = 0;
        isPlayersTurn = false;
        isEnemiesTurn = false;
        isAmbush = false;
    }

    public static void Ambush()
    {
        isAmbush = true;
        isEnemiesTurn = true;
        isPlayersTurn = false;
    }

    public static void StartCombat()
    {
        ResetTurn();
        // Determine if it should start as an ambush or not
        if (isAmbush)
        {
            isEnemiesTurn = true;
        }
        else
        {
            isPlayersTurn = true; // Players start normally
        }
    }

    public static void EndCombat()
    {
        // Clean up combat state, potentially calculate rewards or outcomes
        AppConfig config = ConfigManager.LoadConfig();
        foreach (Character character in config.Parties[config.selectedPartyIndex].Members)
        {
            foreach (Skill skill in character.Skills)
            {
                skill.Cooldown = skill.BaseCooldown; // Reset all skill cooldowns to zero
            }
        }
        ResetTurn();
        ConfigManager.SaveConfig(config);
    }

    public static bool IsCombatOver()
    {
        // Placeholder logic to determine if combat should end
        return false; // Actual implementation needed
    }

    public static void changeParty()
    {
    }
}
