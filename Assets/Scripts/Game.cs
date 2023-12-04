using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject meeplePrefab;
    public GameObject[] squares;
    public List<GameObject> challengeCardsEasy;
    public List<GameObject> challengeCardsMedium;
    public List<GameObject> challengeCardsHard;
    public List<GameObject> eventCardsEarly;
    public List<GameObject> eventCardsMid;
    public List<GameObject> eventCardsLate;
    public List<GameObject> mentorCards;
    public List<GameObject> rulesPages;
    public GameObject difficultySelectionPanel;
    public GameObject earlyEventSelectionPanel;
    public GameObject midEventSelectionPanel;
    public GameObject lateEventSelectionPanel;
    public GameObject challengePanel;
    public GameObject eventPanel;
    public GameObject menuPanel;
    public GameObject rulesPanel;
    public GameObject endingPanel;
    public GameObject mentorPanel;
    public GameObject mentorIcon;
    public Button rollButton;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button revealButton;
    public Button challengeCorrectButton;
    public Button challengeIncorrectButton;
    public Button drawEarlyEventButton;
    public Button drawMidEventButton;
    public Button drawLateEventButton;
    public Button acknowledgeEventButton;
    public Button nextPageButton;
    public Button previousPageButton;
    public Button checkMentorButton;
    public Button mentorBackButton;
    public Button useMentorButton;
    public Button rollChoiceOneButton;
    public Button rollChoiceTwoButton;
    public TMP_Text rollChoiceOneButtonText;
    public TMP_Text rollChoiceTwoButtonText;
    public TMP_Text challengePointsText;
    public TMP_Text turnText;
    public TMP_Text statsText;
    public TMP_Text rollChoiceHintText;

    private List<GameObject> communityCards = new List<GameObject>();
    private List<GameObject> meeples = new List<GameObject>();
    private List<GameObject> activeMeeples = new List<GameObject>();
    private int currentRoll = 0;
    private int currentEventMoveEffect = 0;
    private int currentRulesPageNumber = 0;
    private bool rolled = false;
    private string currentDifficulty = "";
    private bool difficultySelected = false;
    private bool revealed = false;
    private bool eventCardDrawn = false;
    private bool eventAcknowledged = false;
    private bool challengeCorrect = false;
    private bool challengeEvaluated = false;
    private string rollChoiceChosen = "";
    private string mentorToUse = "";
    private GameObject currentMeeple;
    private Color[] meepleColors = {
        Color.red,
        Color.black,
        Color.yellow,
        Color.green,
        Color.blue
    };
    private string[] meepleColorStrings = {
        "Red",
        "Black",
        "Yellow",
        "Green",
        "Blue"
    };
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
        for (int i = 0; i < challengeCardsEasy.Count; i++) {
            challengeCardsEasy[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < challengeCardsMedium.Count; i++) {
            challengeCardsMedium[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < challengeCardsHard.Count; i++) {
            challengeCardsHard[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < eventCardsEarly.Count; i++) {
            eventCardsEarly[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < eventCardsMid.Count; i++) {
            eventCardsMid[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < eventCardsLate.Count; i++) {
            eventCardsLate[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mentorCards.Count; i++) {
            mentorCards[i].gameObject.SetActive(false);
        }
        rollButton.onClick.AddListener(RollOnClick);
        rollButton.gameObject.SetActive(false);
        revealButton.onClick.AddListener(RevealOnClick);
        revealButton.gameObject.SetActive(false);
        acknowledgeEventButton.onClick.AddListener(AcknowledgeEventOnClick);
        drawEarlyEventButton.onClick.AddListener(DrawEarlyOnClick);
        drawMidEventButton.onClick.AddListener(DrawMidOnClick);
        drawLateEventButton.onClick.AddListener(DrawLateOnClick);
        challengeCorrectButton.gameObject.SetActive(false);
        challengeIncorrectButton.gameObject.SetActive(false);
        challengeCorrectButton.onClick.AddListener(ChallengeCorrectOnClick);
        challengeIncorrectButton.onClick.AddListener(ChallengeIncorrectOnClick);
        easyButton.onClick.AddListener(EasyOnClick);
        mediumButton.onClick.AddListener(MediumOnClick);
        hardButton.onClick.AddListener(HardOnClick);
        nextPageButton.onClick.AddListener(NextPageOnClick);
        previousPageButton.onClick.AddListener(PreviousPageOnClick);
        checkMentorButton.onClick.AddListener(CheckMentorOnClick);
        mentorBackButton.onClick.AddListener(MentorBackOnClick);
        useMentorButton.onClick.AddListener(UseMentorOnClick);
        rollChoiceOneButton.onClick.AddListener(RollChoiceOneOnClick);
        rollChoiceTwoButton.onClick.AddListener(RollChoiceTwoOnClick);
        difficultySelectionPanel.gameObject.SetActive(false);
        challengePanel.gameObject.SetActive(false);
        eventPanel.gameObject.SetActive(false);
        earlyEventSelectionPanel.gameObject.SetActive(false);
        midEventSelectionPanel.gameObject.SetActive(false);
        lateEventSelectionPanel.gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        Setup();

        // While there is still a meeple that is unfinished, take a turn
        while (activeMeeples.Count > 0)
        {
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

        Square startSquare = squares[0].GetComponent<Square>();
        for (int i = 0; i < GlobalValues.numPlayers; i++) {
            // Each player chooses a meeple and place it at the path's start
            GameObject meeple = Instantiate(meeplePrefab, new Vector3(startSquare.GetX(), 0, startSquare.GetZ()), Quaternion.identity);
            meeple.GetComponent<Meeple>().SetColor(meepleColors[i]);
            meeple.GetComponent<Meeple>().SetColorString(meepleColorStrings[i]);
            meeple.GetComponent<Meeple>().SetIsPlayer(i == 0);
            meeple.GetComponent<Meeple>().SetCurrentSquare(squares[0]);
            meeple.GetComponent<Meeple>().SetPositionShift(positionShifts[i]);
            meeple.GetComponent<Meeple>().SetName("Player " + (i+1) + " (" + meepleColorStrings[i] + ")");
            meeple.transform.position += positionShifts[i];
            meeples.Add(meeple);
            activeMeeples.Add(meeple);

            // Each player draws a Mentor Card
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
        turnText.SetText("It's " + meeple.GetName() + "'s turn!");
        challengePointsText.SetText("Challenge Points: " + meeple.GetChallengePoints());

        // Register the event
        RegisterEvent("It's " + meeple.GetName() + "'s turn!");
        
        // Play your mentor card if you wish
        if (!meeple.MentorCardUsed()) {
            mentorIcon.gameObject.SetActive(true);
            checkMentorButton.gameObject.SetActive(true);
        } else {
            mentorIcon.gameObject.SetActive(false);
            checkMentorButton.gameObject.SetActive(false);
        }

        // Roll the die or coin according to the life stage
        yield return WaitForRoll();
        RegisterEvent(meeple.GetName() + " rolled a " + currentRoll + ".");
        mentorIcon.gameObject.SetActive(false);
        checkMentorButton.gameObject.SetActive(false);

        // Move the meeple forward
        yield return Move(currentObject, currentRoll);

        Square currentSquare = meeple.GetCurrentSquare().GetComponent<Square>();
        // If you land on an Event Square, draw an Event Card
        if (currentSquare.isEvent) {
            string stage = currentSquare.stage;
            yield return HandleEvent(stage);
        }
        
        // If you land on a Challenge Square, draw a Challenge Card
        if (currentSquare.isChallenge) {
            yield return HandleChallenge();
        }

        // If mentor card gives extra turn
        if (mentorToUse == "Extra Turn") {
            // Roll the die or coin according to the life stage
            yield return WaitForRoll();
            RegisterEvent(meeple.GetName() + " rolled a " + currentRoll + ".");
            mentorIcon.gameObject.SetActive(false);
            checkMentorButton.gameObject.SetActive(false);

            // Move the meeple forward
            yield return Move(currentObject, currentRoll);

            currentSquare = meeple.GetCurrentSquare().GetComponent<Square>();
            // If you land on an Event Square, draw an Event Card
            if (currentSquare.isEvent) {
                string stage = currentSquare.stage;
                yield return HandleEvent(stage);
            }
            
            // If you land on a Challenge Square, draw a Challenge Card
            if (currentSquare.isChallenge) {
                yield return HandleChallenge();
            }
        }

        // Reset current powerup
        mentorToUse = "";
    }

    // Handles when a player lands on an event square
    public IEnumerator HandleEvent(string stage)
    {
        // Check if there is an event card in the deck
        List<GameObject> deck;
        if (stage == "early") {
            deck = eventCardsEarly;
        } else if (stage == "mid") {
            deck = eventCardsMid;
        } else {
            deck = eventCardsLate;
        }
        if (deck.Count > 0) {
            yield return WaitForDrawEvent(stage);
            yield return WaitForAcknowledgeEvent(stage);
            yield return Move(currentMeeple, currentEventMoveEffect);
            string playerName = currentMeeple.GetComponent<Meeple>().GetName();
            if (currentEventMoveEffect < 0) {
                RegisterEvent(playerName + " moved backward " + (currentEventMoveEffect * -1) + " spaces from an event.");
            } else {
                RegisterEvent(playerName + " moved forward " + currentEventMoveEffect + " spaces from an event!");
            }
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

    public IEnumerator DoChallenge(string difficulty)
    {
        challengePanel.gameObject.SetActive(true);
        
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

        // Draw the card
        GameObject currentCard = deck[0];
        deck.RemoveAt(0);
        currentCard.gameObject.SetActive(true);
        deck.Add(currentCard);

        // Let the player answer
        yield return WaitForReveal();
        yield return WaitForChallengeEval();
        string playerName = currentMeeple.GetComponent<Meeple>().GetName();
        if (challengeCorrect) {
            currentMeeple.GetComponent<Meeple>().IncrementChallengePoints(points);
            challengePointsText.SetText("Challenge Points: " + currentMeeple.GetComponent<Meeple>().GetChallengePoints());
            RegisterEvent(playerName + " answered correctly and gained " + points + " Challenge Points!");
        } else {
            RegisterEvent(playerName + " answered incorrectly."); 
        }
        currentCard.gameObject.SetActive(false);
        challengePanel.gameObject.SetActive(false);
    }

    // Waits for the player to click "OK" after reading an event
    public IEnumerator WaitForAcknowledgeEvent(string stage)
    {
        eventPanel.gameObject.SetActive(true);

        // Determine which deck to draw from
        List<GameObject> deck;
        if (stage == "early") {
            deck = eventCardsEarly;
        } else if (stage == "mid") {
            deck = eventCardsMid;
        } else {
            deck = eventCardsLate;
        }

        // Draw the card
        GameObject currentCard = deck[0];
        deck.RemoveAt(0);
        currentCard.gameObject.SetActive(true);
        currentEventMoveEffect = currentCard.GetComponent<EventCard>().moveEffect;
        communityCards.Add(currentCard);

        // Let the player acknowledge
        eventAcknowledged = false;
        while (!eventAcknowledged) {
            yield return null;
        }
        currentCard.gameObject.SetActive(false);
        eventPanel.gameObject.SetActive(false);
    }

    // Waits for the player to click "DRAW" button
    public IEnumerator WaitForDrawEvent(string stage)
    {
        eventCardDrawn = false;
        if (stage == "early") {
            earlyEventSelectionPanel.gameObject.SetActive(true);
        } else if (stage == "mid") {
            midEventSelectionPanel.gameObject.SetActive(true);
        } else {
            lateEventSelectionPanel.gameObject.SetActive(true);
        }
        while (!eventCardDrawn) {
            yield return null;
        }
        if (stage == "early") {
            earlyEventSelectionPanel.gameObject.SetActive(false);
        } else if (stage == "mid") {
            midEventSelectionPanel.gameObject.SetActive(false);
        } else {
            lateEventSelectionPanel.gameObject.SetActive(false);
        } 
    }

    // Waits for the player to click "ANSWER CORRECT" or "ANSWER INCORRECT"
    public IEnumerator WaitForChallengeEval()
    {
        challengeEvaluated = false;
        challengeCorrectButton.gameObject.SetActive(true);
        challengeIncorrectButton.gameObject.SetActive(true);
        while (!challengeEvaluated) {
            yield return null;
        }
        challengeCorrectButton.gameObject.SetActive(false);
        challengeIncorrectButton.gameObject.SetActive(false);
    }

    // Waits for the difficulty to be selected
    public IEnumerator WaitForDifficultySelection()
    {
        difficultySelected = false;
        difficultySelectionPanel.gameObject.SetActive(true);
        while (!difficultySelected) {
            yield return null;
        }
        difficultySelectionPanel.gameObject.SetActive(false);
    }

    // Waits for the reveal button to be clicked
    public IEnumerator WaitForReveal()
    {
        revealed = false;
        revealButton.gameObject.SetActive(true);
        while(!revealed) {
            yield return null;
        }
        revealButton.gameObject.SetActive(false);
    }

    // Waits for the Roll button to be clicked
    public IEnumerator WaitForRoll()
    {
        // if (isUsingAlternateFuture) {
        //     rollButton.gameObject.SetActive(true);

        //     // First roll
        //     rolled = false;
        //     while (!rolled) {
        //         yield return null;
        //     }
        //     int outcomeOne = currentRoll;
        //     rollChoiceOneButton.gameObject.SetActive(true);
        //     rollChoiceOneButtonText.text = "" + currentRoll;
        //     rollChoiceOneButton.enabled = false;
        //     rollChoiceHintText.gameObject.SetActive(true);
        //     rollChoiceHintText.text = "ROLL A SECOND TIME";

        //     // Second roll
        //     rolled = false;
        //     while (!rolled) {
        //         yield return null;
        //     }
        //     int outcomeTwo = currentRoll;
        //     rollChoiceTwoButton.gameObject.SetActive(true);
        //     rollChoiceTwoButtonText.text = "" + currentRoll;
        //     rollChoiceOneButton.enabled = true;
        //     rollChoiceHintText.text = "CHOOSE THE BETTER ROLL";

        //     // Hide the roll button
        //     rollButton.gameObject.SetActive(false);

        //     // Wait for the player's choice between the two rolls
        //     rollChoiceChosen = "";
        //     while (rollChoiceChosen != "") {
        //         yield return null;
        //     }
        //     if (rollChoiceChosen == "one") {
        //         currentRoll = outcomeOne;
        //     } else if (rollChoiceChosen == "two") {
        //         currentRoll = outcomeTwo;
        //     }
        //     rollChoiceOneButton.gameObject.SetActive(false);
        //     rollChoiceTwoButton.gameObject.SetActive(false);
        //     rollChoiceHintText.gameObject.SetActive(false);
        // } else {
            bool rollingTwice = false;
            rolled = false;
            rollButton.gameObject.SetActive(true);
            while(!rolled) {
                // If mentor card gives inspiration boost, they get to do a challenge right now
                if (mentorToUse == "Inspiration Boost") {
                    yield return HandleChallenge();
                    mentorToUse = "";
                }
                // If mentor card gives alternate future, they get to roll twice and choose the better one
                if (mentorToUse == "Alternate Future") {
                    rollingTwice = true;
                }
                yield return null;
            }

            // Handling the second roll if alternate future
            if (rollingTwice) {
                mentorToUse = "";
                int outcomeOne = currentRoll;
                Debug.Log("outcomeOne: " + outcomeOne);
                rollChoiceOneButton.gameObject.SetActive(true);
                rollChoiceOneButtonText.text = "" + currentRoll;
                rollChoiceOneButton.enabled = false;
                rollChoiceHintText.gameObject.SetActive(true);
                rollChoiceHintText.text = "ROLL A SECOND TIME";

                // Second roll
                rolled = false;
                while (!rolled) {
                    yield return null;
                }
                int outcomeTwo = currentRoll;
                rollChoiceTwoButton.gameObject.SetActive(true);
                rollChoiceTwoButtonText.text = "" + currentRoll;
                rollChoiceOneButton.enabled = true;
                rollChoiceHintText.text = "CHOOSE THE BETTER ROLL";

                // Hide the roll button
                rollButton.gameObject.SetActive(false);

                // Wait for the player's choice between the two rolls
                rollChoiceChosen = "";
                while (rollChoiceChosen == "") {
                    yield return null;
                }
                if (rollChoiceChosen == "one") {
                    currentRoll = outcomeOne;
                } else if (rollChoiceChosen == "two") {
                    currentRoll = outcomeTwo;
                }
                rollChoiceOneButton.gameObject.SetActive(false);
                rollChoiceTwoButton.gameObject.SetActive(false);
                rollChoiceHintText.gameObject.SetActive(false);
            }

            rollButton.gameObject.SetActive(false);
        // }
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

        for (int i = 0; i < spaces; i++) {
            // If hit "finish" square while moving, go backwards
            if (currentObject.GetComponent<Meeple>().GetCurrentPathIndex() == 46) {
                direction = -1;
            }

            // Get target position
            int currentPathIndex = currentObject.GetComponent<Meeple>().GetCurrentPathIndex() + direction;
            float endX = squares[currentPathIndex].GetComponent<Square>().GetX();
            float endZ = squares[currentPathIndex].GetComponent<Square>().GetZ();
            Vector3 endPosition = new Vector3(endX, currentObject.transform.position.y, endZ);
            endPosition += currentObject.GetComponent<Meeple>().GetPositionShift();

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
            currentObject.GetComponent<Meeple>().SetCurrentPathIndex(currentPathIndex);
            currentObject.GetComponent<Meeple>().SetCurrentSquare(squares[currentPathIndex]);
            yield return new WaitForSeconds(0.05f);

            // If hit "start" square while moving, just stop
            if (currentObject.GetComponent<Meeple>().GetCurrentPathIndex() == 0) {
                break;
            }
        }

        // If end on "finish" square, the meeple is done
        if (currentObject.GetComponent<Meeple>().GetCurrentPathIndex() == 46) {
            int numDone = GlobalValues.numPlayers - activeMeeples.Count;
            int rp = 6 - numDone;
            currentObject.GetComponent<Meeple>().SetRankPoints(rp);
            string playerName = currentObject.GetComponent<Meeple>().GetName();
            RegisterEvent(playerName + " reached the end of their scientific journey and gained " + rp + " Rank Points!");
        }
    }

    // What happens when the Roll button is clicked
    void RollOnClick()
    {
        int currentPathIndex = currentMeeple.GetComponent<Meeple>().GetCurrentPathIndex();
        string stage = squares[currentPathIndex].GetComponent<Square>().stage;
        int minRoll = 0;
        int maxRoll = 0;
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
    void EasyOnClick()
    {
        currentDifficulty = "easy";
        difficultySelected = true;
    }
    void MediumOnClick()
    {
        currentDifficulty = "medium";
        difficultySelected = true;
    }
    void HardOnClick()
    {
        currentDifficulty = "hard";
        difficultySelected = true;
    }
    void RevealOnClick()
    {
        revealed = true;
    }
    void ChallengeCorrectOnClick()
    {
        challengeCorrect = true;
        challengeEvaluated = true;
    }
    void ChallengeIncorrectOnClick()
    {
        challengeCorrect = false;
        challengeEvaluated = true;
    }
    void DrawEarlyOnClick()
    {
        eventCardDrawn = true;
    }
    void DrawMidOnClick()
    {
        eventCardDrawn = true;
    }
    void DrawLateOnClick()
    {
        eventCardDrawn = true;
    }
    void AcknowledgeEventOnClick()
    {
        eventAcknowledged = true;
    }
    void NextPageOnClick()
    {
        if (currentRulesPageNumber < 3) {
            for (int i = 0; i < rulesPages.Count; i++) {
                rulesPages[i].gameObject.SetActive(false);
            }
            currentRulesPageNumber += 1;
            rulesPages[currentRulesPageNumber].gameObject.SetActive(true);
        }
    }
    void PreviousPageOnClick()
    {
        if (currentRulesPageNumber > 0) {
            for (int i = 0; i < rulesPages.Count; i++) {
                rulesPages[i].gameObject.SetActive(false);
            }
            currentRulesPageNumber -= 1;
            rulesPages[currentRulesPageNumber].gameObject.SetActive(true);
        } 
    }
    void CheckMentorOnClick()
    {
        mentorPanel.gameObject.SetActive(true);
        currentMeeple.GetComponent<Meeple>().GetMentorCard().gameObject.SetActive(true);
    }
    void MentorBackOnClick()
    {
        mentorPanel.gameObject.SetActive(false);
        currentMeeple.GetComponent<Meeple>().GetMentorCard().gameObject.SetActive(false);
    }
    void UseMentorOnClick()
    {
        mentorToUse = currentMeeple.GetComponent<Meeple>().GetMentorCard().GetComponent<MentorCard>().mentorName;

        // Register the event
        string note = "";
        if (mentorToUse == "Inspiration Boost") {
            note = ", and gets to draw a Challenge Card and answer a trivia question!";
        } else if (mentorToUse == "Alternate Future") {
            note = ", and gets to roll twice and choose the better result!";
        } else if (mentorToUse == "Extra Turn") {
            note = ", and gets an extra turn immediately after this one!";
        }
        RegisterEvent(currentMeeple.GetComponent<Meeple>().GetName() + " used their mentor card " + mentorToUse + note);

        // Mark the mentor card as used
        currentMeeple.GetComponent<Meeple>().UseMentorCard();
        mentorIcon.gameObject.SetActive(false);
        checkMentorButton.gameObject.SetActive(false);
        MentorBackOnClick();
    }
    void RollChoiceOneOnClick()
    {
        rollChoiceChosen = "one";
    }
    void RollChoiceTwoOnClick()
    {
        rollChoiceChosen = "two";
    }


    public List<GameObject> ShuffleList(List<GameObject> list)
    {
        List<GameObject> temp = new List<GameObject>();
        List<GameObject> shuffled = new List<GameObject>();
        temp.AddRange(list);
        for (int i = 0; i < list.Count; i++)
        {
            int index = Random.Range(0, temp.Count - 1);
            shuffled.Add(temp[index]);
            temp.RemoveAt(index);
        }
        return shuffled;
    }

    public void RegisterEvent(string s) {
        GlobalValues.events.Add(s);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Show menu panel
            menuPanel.gameObject.SetActive(true);
        }
    }
}

