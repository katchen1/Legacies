using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public GameObject meeplePrefab;
    public GameObject[] squares;

    public List<GameObject> challengeCardsEasy, challengeCardsMedium, challengeCardsHard; // Challenge cards
    public List<GameObject> eventCardsEarly, eventCardsMid, eventCardsLate; // Event cards
    public List<GameObject> mentorCards; // Mentor cards
    public List<GameObject> rulesPages; // Rulebook
    public List<GameObject> communitySlots; // There are 8 community area slots
    
    public List<TMP_Text> groupPlayerTexts, groupResultTexts; // Group challenges
    public List<Button> groupCorrectButtons, groupIncorrectButtons; // Group challenges

    public GameObject difficultySelectionPanel, challengePanel, groupChallengePanel; // Challenge handling
    public GameObject earlyEventSelectionPanel, midEventSelectionPanel, lateEventSelectionPanel, eventPanel; // Event handling
    public GameObject communityPanel, menuPanel, rulesPanel, endingPanel, mentorPanel, mentorIcon, groupEvalHint; // Other UI objects

    public Button easyButton, mediumButton, hardButton, revealButton, challengeCorrectButton, challengeIncorrectButton; // Challenge buttons
    public Button drawEarlyEventButton, drawMidEventButton, drawLateEventButton, acknowledgeEventButton; // Event buttons
    public Button checkMentorButton, mentorBackButton, useMentorButton; // Mentor buttons
    public Button nextPageButton, previousPageButton; // Rulebook buttons
    public Button checkCommunityButton, communityNextButton, communityPrevButton, communityBackButton; // Community area buttons
    public Button rollButton, rollChoiceOneButton, rollChoiceTwoButton; // Roll buttons
    public Button groupRevealButton, groupDoneButton; // Group challenges

    public TMP_Text rollChoiceOneButtonText, rollChoiceTwoButtonText, rollChoiceHintText; // Roll texts
    public TMP_Text turnText, challengePointsText, statsText; // Other UI texts

    // Private variables
    private List<GameObject> communityCards = new List<GameObject>();
    private List<GameObject> meeples = new List<GameObject>();
    private List<GameObject> activeMeeples = new List<GameObject>();
    private List<List<GameObject>> decks = new List<List<GameObject>>();
    private List<bool> groupChallengeResults = new List<bool>();
    private int currentRoll, currentEventMoveEffect, currentRulesPageNumber = 0;
    private int communityPageNumber = 0;
    private bool rolled, eventCardDrawn, eventAcknowledged, difficultySelected, revealed, groupRevealed, groupDone, challengeEvaluated, challengeCorrect = false;
    private string currentDifficulty, rollChoiceChosen, mentorToUse = "";

    // Meeples
    private GameObject currentMeeple;
    private Color[] meepleColors = { Color.red, Color.black, Color.yellow, Color.green, Color.blue };
    private string[] meepleColorStrings = { "Red", "Black",  "Yellow",  "Green",  "Blue" };
    private Vector3[] positionShifts = {
        new Vector3(0f, 0f, 0f),
        new Vector3(0.2f, 0f, 0.2f),
        new Vector3(0.2f, 0f, 0.4f),
        new Vector3(-0.2f, 0f, 0.2f),
        new Vector3(-0.2f, 0f, 0.4f)
    };

    // Start is called before the first frame update
    void Start()
    {
        // Deactivate stuff
        decks = new List<List<GameObject>>{ challengeCardsEasy, challengeCardsMedium, challengeCardsHard, eventCardsEarly, eventCardsMid, eventCardsLate, mentorCards };
        List<GameObject> buttons = new List<GameObject>{ rollButton.gameObject, revealButton.gameObject, groupRevealButton.gameObject, challengeCorrectButton.gameObject, challengeIncorrectButton.gameObject, groupDoneButton.gameObject, groupRevealButton.gameObject };
        List<GameObject> panels = new List<GameObject>{ difficultySelectionPanel, challengePanel, eventPanel, earlyEventSelectionPanel, midEventSelectionPanel, lateEventSelectionPanel, communityPanel, groupChallengePanel };
        DeactivateAllLists(decks);
        DeactivateAllItems(buttons);
        DeactivateAllItems(panels);
        for (int i = 0; i < 4; i++) {
            groupPlayerTexts[i].gameObject.SetActive(false);
            groupResultTexts[i].gameObject.SetActive(false);
            groupCorrectButtons[i].gameObject.SetActive(false);
            groupIncorrectButtons[i].gameObject.SetActive(false);
        }
        groupEvalHint.gameObject.SetActive(false);

        // Assign button listeners
        Button[] clickableButtons = { 
            rollButton, revealButton, acknowledgeEventButton, drawEarlyEventButton, drawMidEventButton, drawLateEventButton,
            challengeCorrectButton, challengeIncorrectButton, easyButton, mediumButton, hardButton, nextPageButton, previousPageButton,
            checkMentorButton, mentorBackButton, useMentorButton, rollChoiceOneButton, rollChoiceTwoButton, checkCommunityButton,
            communityNextButton, communityPrevButton, communityBackButton, groupRevealButton, groupDoneButton,
            groupCorrectButtons[0], groupCorrectButtons[1], groupCorrectButtons[2], groupCorrectButtons[3],
            groupIncorrectButtons[0], groupIncorrectButtons[1], groupIncorrectButtons[2], groupIncorrectButtons[3]
        };
        UnityAction[] listeners = {
            RollOnClick, RevealOnClick, AcknowledgeEventOnClick, DrawEventCardOnClick, DrawEventCardOnClick, DrawEventCardOnClick,
            ChallengeCorrectOnClick, ChallengeIncorrectOnClick, EasyOnClick, MediumOnClick, HardOnClick, NextPageOnClick, PreviousPageOnClick,
            CheckMentorOnClick, MentorBackOnClick, UseMentorOnClick, RollChoiceOneOnClick, RollChoiceTwoOnClick, CommunityOnClick,
            CommunityNextOnClick, CommunityPrevOnClick, CommunityBackOnClick, GroupRevealOnClick, GroupDoneOnClick,
            GroupCorrectOnClick1, GroupCorrectOnClick2, GroupCorrectOnClick3, GroupCorrectOnClick4,
            GroupIncorrectOnClick1, GroupIncorrectOnClick2, GroupIncorrectOnClick3, GroupIncorrectOnClick4
        };
        for(int i = 0; i < 32; i++) {
            clickableButtons[i].onClick.AddListener(listeners[i]);
        }
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        Setup();

        // While there is still a meeple that is unfinished, take a turn
        while (activeMeeples.Count > 0) {
            currentMeeple = activeMeeples[0];
            yield return Turn(currentMeeple);
            activeMeeples.RemoveAt(0);
            if (currentMeeple.GetComponent<Meeple>().GetRankPoints() == 0) {
                // Only queue back the meeples that are undone
                activeMeeples.Add(currentMeeple);
            }
        }

        // End game
        string stats = "RP = Rank Points\nCP = Challenge Points\n\n";
        for (int i = 0; i < meeples.Count; i++) {
            Meeple meeple = meeples[i].GetComponent<Meeple>();
            int cp = meeple.GetChallengePoints();
            int rp = meeple.GetRankPoints();
            int tp = cp + rp;
            stats += meeple.GetName() + "\t\t" + rp + " RP + " + cp + " CP = " + tp + "\n";
        }
        endingPanel.gameObject.SetActive(true);
        statsText.SetText(stats);
    }

    // Set up the game
    public void Setup()
    {
        // Make the player read the rules
        rulesPanel.gameObject.SetActive(true);

        // Shuffle the 3 types of challenge cards, 3 types of event cards, and mentor cards
        challengeCardsEasy = ShuffleList(challengeCardsEasy);
        challengeCardsMedium = ShuffleList(challengeCardsMedium);
        challengeCardsHard = ShuffleList(challengeCardsHard);
        eventCardsEarly = ShuffleList(eventCardsEarly);
        eventCardsMid = ShuffleList(eventCardsMid);
        eventCardsLate = ShuffleList(eventCardsLate);
        mentorCards = ShuffleList(mentorCards);

        // Each player chooses a meeple and place it at the path's start
        Square startSquare = squares[0].GetComponent<Square>();
        for (int i = 0; i < GlobalValues.numPlayers; i++) {
            GameObject meeple = Instantiate(meeplePrefab, new Vector3(startSquare.GetX(), 0, startSquare.GetZ()), Quaternion.identity);
            meeple.GetComponent<Meeple>().SetColor(meepleColors[i]);
            meeple.GetComponent<Meeple>().SetColorString(meepleColorStrings[i]);
            meeple.GetComponent<Meeple>().SetIsPlayer(i == 0);
            meeple.GetComponent<Meeple>().SetCurrentSquare(squares[0]);
            meeple.GetComponent<Meeple>().SetPositionShift(positionShifts[i]);
            meeple.GetComponent<Meeple>().SetName("Player " + (i+1) + " (" + meepleColorStrings[i] + ")");
            meeple.GetComponent<Meeple>().SetPlayerIndex(i);
            meeple.transform.position += positionShifts[i];
            meeples.Add(meeple);
            activeMeeples.Add(meeple);

            // Each player also draws a Mentor Card
            meeple.GetComponent<Meeple>().SetMentorCard(mentorCards[i]);
        }
    }

    // Do a turn
    public IEnumerator Turn(GameObject currentObject)
    {
        yield return new WaitForSeconds(2);

        // Update the current player
        Meeple meeple = currentObject.GetComponent<Meeple>();
        GlobalValues.currentPlayer = meeple;
        challengePointsText.SetText("Challenge Points: " + meeple.GetChallengePoints());
        turnText.SetText("It's " + meeple.GetName() + "'s turn!");
        RegisterEvent("It's " + meeple.GetName() + "'s turn!");
        
        // Display the player's mentor card if it has not been used
        mentorIcon.gameObject.SetActive(!meeple.MentorCardUsed());
        checkMentorButton.gameObject.SetActive(!meeple.MentorCardUsed());

        // Roll the die or coin according to the life stage
        yield return WaitForRoll();
        RegisterEvent(meeple.GetName() + " rolled a " + currentRoll + ".");
        yield return Move(currentObject, currentRoll);

        // Hide the mentor card
        mentorIcon.gameObject.SetActive(false);
        checkMentorButton.gameObject.SetActive(false);

        // Handle events and challenges
        Square currentSquare = meeple.GetCurrentSquare().GetComponent<Square>();
        if (currentSquare.isEvent) {
            yield return HandleEvent(currentSquare.stage);
        } else if (currentSquare.isChallenge) {
            yield return HandleChallenge();
        }

        // If mentor card gives extra turn
        if (mentorToUse == "Extra Turn") {
            // Roll the die or coin according to the life stage
            yield return WaitForRoll();
            RegisterEvent(meeple.GetName() + " rolled a " + currentRoll + ".");
            yield return Move(currentObject, currentRoll);

            // Handle events and challenges
            currentSquare = meeple.GetCurrentSquare().GetComponent<Square>();
            if (currentSquare.isEvent) {
                yield return HandleEvent(currentSquare.stage);
            } else if (currentSquare.isChallenge) {
                yield return HandleChallenge();
            }
        }    
        mentorToUse = "";    
    }

    // Handles when a player lands on an event square
    public IEnumerator HandleEvent(string stage)
    {
        // Determine the deck based on the stage
        List<GameObject> deck;
        GameObject panel;
        if (stage == "early") {
            deck = eventCardsEarly;
            panel = earlyEventSelectionPanel;
        } else if (stage == "mid") {
            deck = eventCardsMid;
            panel = midEventSelectionPanel;
        } else {
            deck = eventCardsLate;
            panel = lateEventSelectionPanel;
        }

        // Check if there are cards left in the deck
        if (deck.Count > 0) {
            yield return WaitForDrawEvent(panel);
            yield return WaitForAcknowledgeEvent(deck);
            yield return Move(currentMeeple, currentEventMoveEffect);

            // Register the move event
            string playerName = currentMeeple.GetComponent<Meeple>().GetName();
            string direction = currentEventMoveEffect < 0? "backward": "forward";
            int numSpaces = currentEventMoveEffect < 0? (currentEventMoveEffect * -1): currentEventMoveEffect;
            RegisterEvent(playerName + " moved " + direction + " " + numSpaces + " spaces from an event.");
        } else {
            RegisterEvent("The " + stage + "-life Event Card deck ran out of cards.");
        }
    }

    // Handles when a player lands on a challenge square
    public IEnumerator HandleChallenge()
    {
        yield return WaitForDifficultySelection();
        yield return DoChallenge(currentDifficulty);
    }

    // Let the player choose a deck to draw from and answer the challenge question
    public IEnumerator DoChallenge(string difficulty)
    {
        // Determine which deck to draw from
        List<GameObject> deck;
        int points;
        if (difficulty == "easy") {
            deck = challengeCardsEasy;
            points = 1;
        } else if (difficulty == "medium") {
            deck = challengeCardsMedium;
            points = 2;
        } else {
            deck = challengeCardsHard;
            points = 3;
        }

        // Draw the card and show it
        GameObject currentCard = deck[0];
        deck.RemoveAt(0);
        deck.Add(currentCard);

        if (currentCard.GetComponent<ChallengeCard>().isGroupChallenge) {
            groupChallengePanel.gameObject.SetActive(true);
            currentCard.gameObject.SetActive(true);

            // Let the players go around and answer
            yield return WaitForGroupChallengeReveal();
            currentCard.gameObject.SetActive(false);
            yield return WaitForGroupChallengeEval();

            // Handle challenge result
            List<string> correctPlayerNames = new List<string>();
            for (int i = 0; i < activeMeeples.Count; i++) {
                Meeple meeple = activeMeeples[i].GetComponent<Meeple>();
                if (groupChallengeResults[meeple.GetPlayerIndex()]) {
                    meeple.IncrementChallengePoints(points);
                    correctPlayerNames.Add(meeple.GetName());
                }
            }

            // Register the group challenge event on the log
            string displayText = "";
            if (correctPlayerNames.Count == 0) {
                displayText = "No one";
            } else if (correctPlayerNames.Count == 1) {
                displayText = correctPlayerNames[0];
            } else if (correctPlayerNames.Count == 2) {
                displayText = correctPlayerNames[0] + " and " + correctPlayerNames[1];
            } else {
                for (int i = 0; i < correctPlayerNames.Count - 1; i++) {
                    displayText += correctPlayerNames[i] + ", ";
                }
                displayText += "and " + correctPlayerNames[correctPlayerNames.Count - 1];
            }
            displayText += " answered the Group Challenge correctly and gained " + points + " points!";
            RegisterEvent(displayText);
        } else {
            challengePanel.gameObject.SetActive(true);
            currentCard.gameObject.SetActive(true);

            // Let the player answer
            yield return WaitForReveal();
            yield return WaitForChallengeEval();

            // Handle challenge result
            string playerName = GlobalValues.currentPlayer.GetName();
            if (challengeCorrect) {
                GlobalValues.currentPlayer.IncrementChallengePoints(points);
                RegisterEvent(playerName + " answered correctly and gained " + points + " Challenge Points!");
            } else {
                RegisterEvent(playerName + " answered incorrectly."); 
            }
        }

        // Hide the card and challenge panel
        challengePointsText.SetText("Challenge Points: " + GlobalValues.currentPlayer.GetChallengePoints());
        currentCard.gameObject.SetActive(false);
        challengePanel.gameObject.SetActive(false);
    }

    // Waits for the player to click "OK" after reading an event
    public IEnumerator WaitForAcknowledgeEvent(List<GameObject> deck)
    {
        // Draw the card and show it
        GameObject currentCard = deck[0];
        deck.RemoveAt(0);
        eventPanel.gameObject.SetActive(true);
        currentCard.gameObject.SetActive(true);
        currentEventMoveEffect = currentCard.GetComponent<EventCard>().moveEffect;
        communityCards.Add(currentCard);

        // Let the player acknowledge
        eventAcknowledged = false;
        while (!eventAcknowledged) yield return null;
        currentCard.gameObject.SetActive(false);
        eventPanel.gameObject.SetActive(false);
    }

    // Waits for the player to click "DRAW" button
    public IEnumerator WaitForDrawEvent(GameObject selectionPanel)
    {
        eventCardDrawn = false;
        selectionPanel.gameObject.SetActive(true);
        while (!eventCardDrawn) yield return null;
        selectionPanel.gameObject.SetActive(false);
    }

    // Waits for the player to click "ANSWER CORRECT" or "ANSWER INCORRECT"
    public IEnumerator WaitForChallengeEval()
    {
        challengeEvaluated = false;
        challengeCorrectButton.gameObject.SetActive(true);
        challengeIncorrectButton.gameObject.SetActive(true);
        while (!challengeEvaluated) yield return null;
        challengeCorrectButton.gameObject.SetActive(false);
        challengeIncorrectButton.gameObject.SetActive(false);
    }

    // Waits for all players to click "ANSWER CORRECT" or "ANSWER INCORRECT"
    public IEnumerator WaitForGroupChallengeEval()
    {
        groupDone = false;

        // Show the eval interface
        groupEvalHint.gameObject.SetActive(true);
        groupDoneButton.gameObject.SetActive(true);
        for (int i = 0; i < activeMeeples.Count; i++) {
            int playerIndex = activeMeeples[i].GetComponent<Meeple>().GetPlayerIndex();
            groupPlayerTexts[playerIndex].gameObject.SetActive(true);
            groupResultTexts[playerIndex].gameObject.SetActive(true);
            groupResultTexts[playerIndex].GetComponent<TMP_Text>().text = "";
            groupCorrectButtons[playerIndex].gameObject.SetActive(true);
            groupIncorrectButtons[playerIndex].gameObject.SetActive(true);
        }
        groupChallengeResults = new List<bool>{false, false, false, false};
        while (!groupDone) yield return null;

        // Hide the eval interface
        groupEvalHint.gameObject.SetActive(false);
        groupDoneButton.gameObject.SetActive(false);
        for (int i = 0; i < activeMeeples.Count; i++) {
            int playerIndex = activeMeeples[i].GetComponent<Meeple>().GetPlayerIndex();
            groupPlayerTexts[playerIndex].gameObject.SetActive(false);
            groupResultTexts[playerIndex].gameObject.SetActive(false);
            groupCorrectButtons[playerIndex].gameObject.SetActive(false);
            groupIncorrectButtons[playerIndex].gameObject.SetActive(false);
        }
    }

    // Waits for the difficulty to be selected
    public IEnumerator WaitForDifficultySelection()
    {
        difficultySelected = false;
        difficultySelectionPanel.gameObject.SetActive(true);
        while (!difficultySelected) yield return null;
        difficultySelectionPanel.gameObject.SetActive(false);
    }

    // Waits for the reveal button to be clicked
    public IEnumerator WaitForReveal()
    {
        revealed = false;
        revealButton.gameObject.SetActive(true);
        while(!revealed) yield return null;
        revealButton.gameObject.SetActive(false);
    }

    // Waits for the group challenge reveal button to be clicked
    public IEnumerator WaitForGroupChallengeReveal()
    {
        groupRevealed = false;
        groupRevealButton.gameObject.SetActive(true);
        while(!groupRevealed) yield return null;
    }

    // Waits for the Roll button to be clicked
    public IEnumerator WaitForRoll()
    {
        bool rollingTwice = false;

        // First roll
        rolled = false;
        rollButton.gameObject.SetActive(true);
        while(!rolled) {
            // If mentor card gives inspiration boost, they get to do a challenge right now
            if (mentorToUse == "Inspiration Boost") {
                yield return HandleChallenge();
                mentorToUse = "";
            }
            // If mentor card gives alternate future, they get to roll twice and choose the better one
            rollingTwice = mentorToUse == "Alternate Future";
            yield return null;
        }

        // Handling the second roll if alternate future
        if (rollingTwice) {
            mentorToUse = "";
            int outcomeOne = currentRoll;
            rollChoiceOneButton.gameObject.SetActive(true);
            rollChoiceOneButtonText.text = "" + currentRoll;
            rollChoiceOneButton.enabled = false;
            rollChoiceHintText.gameObject.SetActive(true);
            rollChoiceHintText.text = "ROLL A SECOND TIME";

            // Second roll
            rolled = false;
            while (!rolled) yield return null;
            int outcomeTwo = currentRoll;
            rollChoiceTwoButton.gameObject.SetActive(true);
            rollChoiceTwoButtonText.text = "" + currentRoll;
            rollChoiceOneButton.enabled = true;
            rollChoiceHintText.text = "CHOOSE THE BETTER ROLL";
            rollButton.gameObject.SetActive(false);

            // Wait for the player's choice between the two rolls
            rollChoiceChosen = "";
            while (rollChoiceChosen == "") yield return null;
            currentRoll = rollChoiceChosen == "one"? outcomeOne: outcomeTwo;
            rollChoiceOneButton.gameObject.SetActive(false);
            rollChoiceTwoButton.gameObject.SetActive(false);
            rollChoiceHintText.gameObject.SetActive(false);
        }
        rollButton.gameObject.SetActive(false);
    }

    // Moves the currentObject (a Meeple) by currentRoll number of spaces on the board
    public IEnumerator Move(GameObject currentObject, int spaces)
    {
        // Determine direction
        int direction = 1;
        if (spaces < 0) {
            direction = -1;
            spaces *= -1;
        }

        Meeple meeple = currentObject.GetComponent<Meeple>();
        for (int i = 0; i < spaces; i++) {
            // If hit "finish" square while moving, go backwards
            if (meeple.GetCurrentPathIndex() == 46) direction = -1;

            // Get target position
            int currentPathIndex = meeple.GetCurrentPathIndex() + direction;
            float endX = squares[currentPathIndex].GetComponent<Square>().GetX();
            float endZ = squares[currentPathIndex].GetComponent<Square>().GetZ();
            Vector3 endPosition = new Vector3(endX, currentObject.transform.position.y, endZ);
            endPosition += meeple.GetPositionShift();

            // Move the meeple to the next square in one second
            float elapsedTime = 0f;
            float seconds = 0.2f;
            Vector3 startPosition = currentObject.transform.position;
            while (elapsedTime < seconds) {
                currentObject.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            currentObject.transform.position = endPosition;
            meeple.SetCurrentPathIndex(currentPathIndex);
            meeple.SetCurrentSquare(squares[currentPathIndex]);
            yield return new WaitForSeconds(0.05f);

            // If hit "start" square while moving, just stop
            if (currentObject.GetComponent<Meeple>().GetCurrentPathIndex() == 0) break;
        }

        // If end on "finish" square, the meeple is done
        if (meeple.GetCurrentPathIndex() == 46) {
            int numDone = GlobalValues.numPlayers - activeMeeples.Count;
            int rp = 6 - numDone;
            meeple.SetRankPoints(rp);
            RegisterEvent(meeple.GetName() + " reached the end of their scientific journey and gained " + rp + " Rank Points!");
        }
    }

    // What happens when the Roll button is clicked
    void RollOnClick()
    {
        int currentPathIndex = GlobalValues.currentPlayer.GetCurrentPathIndex();
        string stage = squares[currentPathIndex].GetComponent<Square>().stage;
        int minRoll = 0, maxRoll = 0;
        if (stage == "early") {
            minRoll = 1;
            maxRoll = 4;
        } else if (stage == "mid") {
            minRoll = 1;
            maxRoll = 6;
        } else if (stage == "late") {
            minRoll = 1;
            maxRoll = 2;
        }
        currentRoll = Random.Range(minRoll, maxRoll + 1);
        rolled = true;
    }

    // Other button on click handlers
    void EasyOnClick() {
        currentDifficulty = "easy";
        difficultySelected = true;
    }
    void MediumOnClick() {
        currentDifficulty = "medium";
        difficultySelected = true;
    }
    void HardOnClick() {
        currentDifficulty = "hard";
        difficultySelected = true;
    }
    void RevealOnClick() {
        revealed = true;
    }
    void GroupRevealOnClick() {
        groupRevealed = true;
        communityPanel.gameObject.SetActive(true);
    }
    void ChallengeCorrectOnClick() {
        challengeCorrect = true;
        challengeEvaluated = true;
    }
    void ChallengeIncorrectOnClick() {
        challengeCorrect = false;
        challengeEvaluated = true;
    }
    void DrawEventCardOnClick() {
        eventCardDrawn = true;
    }
    void AcknowledgeEventOnClick() {
        eventAcknowledged = true;
    }
    void NextPageOnClick() {
        if (currentRulesPageNumber < 3) {
            DeactivateAllItems(rulesPages);
            currentRulesPageNumber += 1;
            rulesPages[currentRulesPageNumber].gameObject.SetActive(true);
        }
    }
    void PreviousPageOnClick() {
        if (currentRulesPageNumber > 0) {
            DeactivateAllItems(rulesPages);
            currentRulesPageNumber -= 1;
            rulesPages[currentRulesPageNumber].gameObject.SetActive(true);
        } 
    }
    void CheckMentorOnClick() {
        mentorPanel.gameObject.SetActive(true);
        GlobalValues.currentPlayer.GetMentorCard().gameObject.SetActive(true);
    }
    void MentorBackOnClick() {
        mentorPanel.gameObject.SetActive(false);
        GlobalValues.currentPlayer.GetMentorCard().gameObject.SetActive(false);
    }
    void UseMentorOnClick() {
        mentorToUse = GlobalValues.currentPlayer.GetMentorCard().GetComponent<MentorCard>().mentorName;

        // Register the event
        string note = "";
        if (mentorToUse == "Inspiration Boost") {
            note = ", and gets to draw a Challenge Card and answer a trivia question!";
        } else if (mentorToUse == "Alternate Future") {
            note = ", and gets to roll twice and choose the better result!";
        } else if (mentorToUse == "Extra Turn") {
            note = ", and gets an extra turn immediately after this one!";
        }
        RegisterEvent(GlobalValues.currentPlayer.GetName() + " used their mentor card " + mentorToUse + note);

        // Mark the mentor card as used and close the panel
        GlobalValues.currentPlayer.UseMentorCard();
        mentorIcon.gameObject.SetActive(false);
        checkMentorButton.gameObject.SetActive(false);
        MentorBackOnClick();
    }
    void RollChoiceOneOnClick() {
        rollChoiceChosen = "one";
    }
    void RollChoiceTwoOnClick() {
        rollChoiceChosen = "two";
    }
    void CommunityOnClick() {
        communityPanel.gameObject.SetActive(true);
        for (int i = 0; i < 8; i++) {
            int index = communityPageNumber * 8 + i;
            if (index < communityCards.Count) {
                communitySlots[i].GetComponent<RawImage>().texture = communityCards[i].GetComponent<RawImage>().texture;
            }
        }
    }
    void CommunityNextOnClick() {
        float numPages = communityCards.Count / 8f;
        if (communityPageNumber < numPages - 1) {
            communityPageNumber += 1;
            for (int i = 0; i < 8; i++) {
                int index = communityPageNumber * 8 + i;
                communitySlots[i].GetComponent<RawImage>().texture = null;
                if (index < communityCards.Count) {
                    communitySlots[i].GetComponent<RawImage>().texture = communityCards[index].GetComponent<RawImage>().texture;
                }
            }
        }
    }
    void CommunityPrevOnClick() {
        if (communityPageNumber > 0) {
            communityPageNumber -= 1;
            for (int i = 0; i < 8; i++) {
                int index = communityPageNumber * 8 + i;
                if (index < communityCards.Count) {
                    communitySlots[i].GetComponent<RawImage>().texture = communityCards[i].GetComponent<RawImage>().texture;
                }
            }
        }
    }
    void CommunityBackOnClick() {
        communityPanel.gameObject.SetActive(false);
    }
    void GroupDoneOnClick() {
        groupDone = true;
    }
    void GroupCorrectOnClick1() {
        groupChallengeResults[0] = true;
        groupResultTexts[0].GetComponent<TMP_Text>().text = "CORRECT";
    }
    void GroupCorrectOnClick2() {
        groupChallengeResults[1] = true;
        groupResultTexts[1].GetComponent<TMP_Text>().text = "CORRECT";
    }
    void GroupCorrectOnClick3() {
        groupChallengeResults[2] = true;
        groupResultTexts[2].GetComponent<TMP_Text>().text = "CORRECT";
    }
    void GroupCorrectOnClick4() {
        groupChallengeResults[3] = true;
        groupResultTexts[3].GetComponent<TMP_Text>().text = "CORRECT";
    }
    void GroupIncorrectOnClick1() {
        groupChallengeResults[0] = false;
        groupResultTexts[0].GetComponent<TMP_Text>().text = "INCORRECT";
    }
    void GroupIncorrectOnClick2() {
        groupChallengeResults[1] = false;
        groupResultTexts[1].GetComponent<TMP_Text>().text = "INCORRECT";
    }
    void GroupIncorrectOnClick3() {
        groupChallengeResults[2] = false;
        groupResultTexts[2].GetComponent<TMP_Text>().text = "INCORRECT";
    }
    void GroupIncorrectOnClick4() {
        groupChallengeResults[3] = false;
        groupResultTexts[3].GetComponent<TMP_Text>().text = "INCORRECT";
    }
    

    // Shuffles a list of game objects
    public List<GameObject> ShuffleList(List<GameObject> list)
    {
        List<GameObject> temp = new List<GameObject>();
        List<GameObject> shuffled = new List<GameObject>();
        temp.AddRange(list);
        for (int i = 0; i < list.Count; i++) {
            int index = Random.Range(0, temp.Count - 1);
            shuffled.Add(temp[index]);
            temp.RemoveAt(index);
        }
        return shuffled;
    }

    // Registers an event to be shown on the log
    public void RegisterEvent(string s) {
        GlobalValues.events.Add(s);
    }

    // Deactivate all lists in a list
    public void DeactivateAllLists(List<List<GameObject>> l) {
        for (int i = 0; i < l.Count; i++) {
            DeactivateAllItems(l[i]);
        }
    }

    // Deactivates all items in a list
    public void DeactivateAllItems(List<GameObject> l) {
        for (int i = 0; i < l.Count; i++) {
            l[i].gameObject.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Show menu panel
            menuPanel.gameObject.SetActive(true);
        }
    }
}
