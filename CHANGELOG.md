## [v10.0] - 29/09/2025

### Added
- Upgraded project from .NET 4.8 to 8.0, giving a ~35% raw performance increase.
- Added PVS Search to the NegaMax Algorithm.
- Enhanced Aspiration Windows, with variable width depending on depth & score, one-side widening, and gradual widening for deep searches.
- Improved Mechanisms and tweaked values of Base Move Orderer.
- Added MVV-LVA move ordering to capture moves in the NegaMax search.
- Reversed static conditions for Null Move Pruning, with a slight window for StandPat.
- Piece Heat Map values are now calculated 'on the fly' after each move (presuming the material count is great enough), instead of for every Evaluate() call.
- Added more sophisticated AI settings modifier in Analysis Mode, allowing greater control of the AI.
- Added endgame heat map tables to each of the pieces, interpolating between them as the material count drops.
- Added bitboards for the pawns, allowing for detection of past pawns, isolated pawns, and doubled pawns in the evaluation function.
- Adjusted Piece Heat Map values.
- Gives the AI longer to search if we are currently in a recently-broken Aspiration Window, if there are lots of captures and checks available, or if the user made the move that the AI predicted is the best move (sign of a strong opponent).
- Gives the AI more time to think on the moves after pre-moves (ie: book / forced moves), to get the TranspositionTable better calibrated.
- Small AdvancedSearchTime improvements & tweaks (to make moves more human).
- Added 50 move rule draw condition, to both the UI and also the AI's thought process.
- Added compatability with more move & FEN inputs, allowing the user to enter a hybrid of a FEN, + some moves that stem from that position.
- Made "Automatically Reset Game" more advanced, such that if a new FEN is accepted, and the AI plays on that position, the board will be reset to that new FEN.
- Added more generality to AI controls, for better use in interacting with other programs (eg: VersionComparer).
- Added Transposition Table caching to thorough move ordering system, and resets this ordering if settings are changed.
- Added more flexibility and support for entering FENs into the system, meaning one can neglect information, or place it in the wrong order, and have the position still be updated.
- Replaced KingCheck images with a cleaner circular design on the check square.
- Subtle Tweaks to the StartingDepth algorithm.
- Subtle Tweaks to AnimateBoard algorithm.
- Updates to AI difficulty levels, including more modifications to core AI settings.
- Added AI Eval to Puzzle Mode.
- Added the ability to drag the MainMenu form around.
- Now brings forward the settings window if the form is already open when the user clicks the 'Settings' button.
- Sorted general project file organisation.
- Subtle Design Tweaks.
- General Code Condensing, Refactoring, and Efficiency Improvements :D.

### Fixed
- Fixed Aspiration Window break move causing blunders if it is the only move searched (ie: stops it from being pushed to the top).
- Fixed TT Entries not being reset correctly if ABORT is caused in a Null Move branch (which would flag the position as still being a repetition, causing blunders, or storing the wrong move inside the search upon ABORTing).
- Fixed Null Move Pruning interactions with Zobrist Hashing (forgetting to 'take back' null move), causing Transposition Errors.
- Fixed SortMovesThourough approach to how move affects PieceHeatMaps.
- Fixed Thorough Move Orderer incorrectly labelling check moves.
- Fixed AI console outputting in AdvancedSearchTime mode not reflecting the current position's timings.
- Fixed Three-fold repetition working in AI when Transposition Table is turned off.
- Fixed EnPassant extreme values with FEN entering.
- Fixed PGN not updating for forced moves.
- Fixed EnPassant Pin checks.
- Fixed Transposition Table entries not being re-replaced fully in case of ABORT-ing.
- Fixed Double Check errors stemming from corner of board.
- Fixed rook & pawn moves not intercepting castling.
- Made Board Edit Discard button only remove kings, not reset kings to starting position.
- Allowed Remote Mode to be played across a greater range of displays, including those that are skewed in Windows Settings.
- Fixed Opening Book moves not being found on positions that have an altered full-move or half-move count.
- Fixed PGN output not displaying correctly (esp with Undoing a move).



## [v9.0] - 27/12/2024

### Added
- New Game Mode called Remote Mode, which locates any Chess Interface found on the user's screen (except this program! ;D), then connects to it by locating & storing all the pieces.
  The user is then able to play on this interface, either using their mouse or automatically via my AI, and the program will move the user's mouse instantly to play their (or the AI's) moves on this Interface.
  This algorithm is able to detect a move of changes between the interface and its own position, and make the move on the GUI automatically to calibrate the two positions.
- Allowed Remote Mode to connect to any Interface, no matter the size, location, board colours / themes (to a limit), piece designs, Windows Scaling Sizes, etc.
- Added Pawn Promotion Functionality to Remote Mode, along with predicting the Pawn Promotion Pieces, when none / few are available.
- Added Robustness to playing the program's move on the Interface (ie: failures to make the move by taking control of the mouse are detected & repaired automatically).
- To use Remote Play on Lichess:
1) Set your 'Board Theme' to one of 'static' Square Colours (ie: no 'texture' to the squares).
2) Turn off Piece Animations.
3) Turn off Board Highlights.
4) Toggle 'Promote to Queen' to 'Never', or to 'When Pre-moving'.
5) Toggle 'Claim Draw on Three-Fold Repetition' to 'Always'.

- Rewritten AI using the NegaMax algorithm.
- Option Strict Performance Increases.
- Improved Speed via Global PieceMoves in the PieceLegalMoveGenerators Class.
- Improved Move Ordering System for the Base Position. Takes into account the Difference in Value between pieces involved in a capture (as well as prioritising high-value captures - ie: MVV-LVA), Captures being Trades vs open, Pawns Promoting (and almost promoting, along with what piece it promoted to), Pieces closing in on the enemy King, the move resulting in a Check, the new square being controlled by any piece (especially an enemy pawn), how the static evaluation changes from that move, and whether the new move is in the Transposition Table (denoting potentially 'interesting' moves).

### New AI Additions:
- Null Move Pruning.
- Check Extensions.
- Late Move Reductions & Internal Iterative Reductions.
- Delta Pruning Improvements to Quiescence.
- Aspiration Windows to Search.
- Modularity of AI Features, including being able to change the settings / functionality of the aforementioned features.
- Advanced, more human, search time added to AI, for 1 Player & Remote Modes. Takes into account the # of legal moves, allows for pre-moves & long thinks, more realistic book & forced move times, and search extensions / reductions if the AI's latest move failed low / high.
- Edited AI strengths for each of the 1 Player Modes (by making certain AI features modular, and turning them off for the Beginner Difficulties, and furthermore changing TimeForSearch for each difficulty).
- Decreased Time Between AI Moves (TimeToLive -> Generation, and Refactored Checkerboard Refreshing & Painting)
- Functionality for the Board to Morph (via Piece Animations) from one FEN to Another, using the Greedy Algorithm. Used in the Main Program, along with moving between Puzzles, Training Positions, etc.
- Updated Piece Animations.
- Improved BoardEditMode: Added button that Discards the Current Position into one containing just the kings, Copies the user's piece when they press CTRL (even if that piece is still), Initial Position defaults to that of the Current Position, for easy FEN editing.
- Made PGN Importing more Robust, and able to handle more Formats (including being able to copy in an entire game from Lichess, and have the PGN be isolated & played out).
- Smoother & Faster Main Menu Opening Animation.
- Added more Functionality to the Move Structure (more modular & Object Oriented).
- Added Program Name Changes, depending on the current status of the game.
- Changed UserTimeBar Time Scaling & Colouring (better reflects the program's ability).
- Reworked Displaying of a Position's Evaluation on the Console & GUI (no longer symmetric).
- Small tweaks to ReturnPieceValue (fewer calls, more robust).
- Moved many objects to their own, localised Structures, for clarity purposes.
- General Code Condensing, Refactoring, and Efficiency Improvements :D.

### Fixed
- Fixed Transposition Table instability with calculating the fastest Checkmating Sequence.
- Fixed Trade Captures not being sorted under Winning Captures (this has made an insane difference in performance - I can't believe I've caught this so late!!).
- Fixed Invisible Piece Toggling not working.
- Improved robustness of invalid FENs (empty spaces are now replaced with " ")
- Fixed Disappearing kings in BoardEditMode.
- Fixed Dodgy EnPassant rules in BoardEditMode.
- Removed KillerMoves during Quiescence (slight performance increases).
- Fixed En Passant Capture removing checks.
- Made EnPassant a proper Capture Move (now works in Quiescience).
- Make it easier to remove piece in Board Edit Mode, when they are dragged to negative coordinates.
- Fixed logic with detecting TerribleMoves Moves.
- Re-prioritised King Captures in move ordering system (the King now has the value of the smallest piece, so that King Captures are slightly de-prioritised, whilst still prioritising high-value Captures.
- Fixed Hammad AI Mode Compatibility with New AI Features.



## [v8.3] - 16/02/2024

### Added
- King Endgame Heuristic is now stored as a large, pre-computed look-up table, so that values do not need to be calculated for every position (40% efficiency gain over old method).
- Added ability to promote to a knight (by holding down SHIFT during promotion).
- New 'Board Edit' Feature, allowing for easy creation of custom positions.
- 'Click-Move' Feature, where the user is able to make a move by clicking on a piece, followed by its destination.
- Improved King Endgame Heuristic Algorithm (more detailed 'Manhattan Distance').
- Added AI Version to Credits.
- Added an automatic game resetter to Endless AI mode, so that the AI can play forever.
- Redesigned PictureBox system to allow for infinitely many of each piece to be on the board at once (allowing for custom games).
- Decreased time between AI moves (30ms).
- Reworked Opening Animation.
- Updated Puzzle Database.
- Moved 'Images' & 'Sounds' folders to 'Assets'.
- Improved Hammad Mode End-State Responce Algorithm.
- Moved many constants to the GlobalConstants class.
- Refined Settings General Options.
- Added Optional 'Speed' Mode to AI Puzzle Solving (AIWaitAfterPuzzleComplete).
- Small Changes to AI chosen Move output (main change: outputs number of win sequences, rather than overall checkmates).
- Chess.vb is now broken down into further partial classes.
- byVal to byRef changes.
- General Code Condensing :D.

### Fixed
- Fixed bug where MainMenu would become unresponsive when the Credits button is clicked.
- Improved Piece Drop Smoothness (when piece is dropped, it no longer 'glitches out' before disappearing).
- Puzzles now include pawn promotions to pieces other than the Queen.
- AI Stats are now reset when the 'Reset Program' button is pressed.
- If the Opening Book has not loaded by the time the user enters a mode, the Opening Book is no longer passed through the system.
- Fixed bugs with MoveConverter collisions.
- Fixed NodeCount per Second Calculation.
- Fixed Sounds not playing correctly on some devices (length of audio too short).
- Added EnPassant Square Error Detection.
- Increased Size of AI Lifetime Stats (prevents mass crashing after a while).
- Fixed invalid Move Numbers in Puzzle FENs.



## [v8.2] - 04/09/2023

### Added
- AI Moves updated to BitMoves (2x Performance Increase) :D.
- Added PGNs to GameHistory.
- Added AutoPGN outputter to One & Two Player Modes.
- Checks are now added to the PGN exporter.
- Re-implementation of MouseUp() subroutine.
- InCheck, KPos, and EnPassant updated to BitMoves.
- Added 'Node Test' feature to Analysis Mode.
- Restructured TranspositionTable to improve memory usage (stored as a static array, instead of a list of nodes). Implemented using the 'Deep' Replacement scheme (strict replacement).
- Added TimeToLive attribute to TranspositionTable nodes, to improve the long-term efficiency of the 'Deep' Replacement scheme.
- Ensures that the nodes involved in the AI's best line are kept alive in the TranspositionTable (using TimeToLive).
- Updated main AI FOR loops to include the variable type of the incrementing variable.
- Added constraint information to PGN move generator.
- Refined Memory Management Tools.
- Improved Hammad Mode Base Level Algorithm.
- Reduced Opening Book load times (using GC Settings).
- General code condensing & Refactoring :).
- Tweaked PieceHeatMaps.

### Fixed
- Fixed bug with MakeMove not working correctly with black castling.
- Fixed bug with black not being able to checkmate correctly.
- Transposition Table now resets if the core AI settings have been changed mid-game (ie: Quiescence, PieceHeatMaps, Hammad Mode, ...).
- Only one Settings Form can be opened at a time.
- Fixed Bug with OpeningBook not working correctly for Upper Bound.
- Added Cancel button to GameOver screen, so that the user is able to copy the final PGN / FEN.



## [v8.1] - 03/08/2023

### Added
- 'king dead' text for Hammad Mode Checkmate.
- Toggle to disable non-essential GUI elements when the AI is searching (improves low TimeForSearch situations).
- More Compact & Refined Debug Info Console Output.
- Added AI Method to calculate the length of the TranspositionTable Entry (List), which contains the most entries (used to determine how 'full' the TranspositionTable is).
- Option for the AI to search on the position on the user's turn (which fills the TranspositionTable in preparation for the AI's move) - 1-Player Mode only.
- TT data is retained upon ABORTing a search, meaning that some positions can be accessed quicker.
- Restructuring of how BasePieceMoves is generated (contains only legal moves vs pseudo-legal moves).
- Restructuring of how the main Chess.vb form interacts with pieces (due to restructuring of BasePieceMoves).
- Option to output the details of the Current Move the AI is searching on (OutputMoveDebugInfo in SearchSettings) - toggles when the searching process becomes lengthy.
- Added 'VFast' Animation Speed Option (and set default speed to 'Fast').
- General Code Condensing :).

### Fixed
- (pretty much) NO INSTABILITY IN THE TRANSPOSITION TABLE WOOOOOOOOOOOO LET'S GOOOOOO, HELLL YEAHHHHHH :DDDDDDDDDDDDDD - FINALLY!!
- Fixed PictureBox Transparency Issue (with PictureBoxes not overlapping correctly) - finally!!
- Smoother PictureBox movement on the GUI - no stuttering or flickering :))).
- Bug fixes for a single search not having enough time to complete.
- Fixed hidden MainMenu bug.
- Fixed 'Sound Off' Toggle so that it works with Puzzle Mode.
- Board Debug Info now flips in black's favour.
- Fixed bug where checkmate scores are incorrectly retrieved from the Transposition Table.
- Memory Management tools to help prevent the system from crashing during long searches.
- Error handling for 'Move Practice' Training Mode not being able to find a valid position.



## [v8.0] - 14/07/2023

Puzzle mode, with AI implementation.  
Transposition Table redesign & major bug fixes (but still has instability). D:  
Maintaining Transposition Table across multiple moves.  
AI perfomance increases.  
Many, many bug fixes.  
Move list in CreateMoves & PieceLegalMoves with shrunk array.  
Redesigned, larger opening book with weighed moves & faster load times.  
Integer Evaluations (rather than decimal ones).  
Tweeked PieceHeatMaps.  
Uses last (ABORTed) search iteration for Best Move.  
Extra information for AI moves, along with Lifetime Stats.  
Best Path Output (using Transposition Table).  



## [v7.0] - 10/04/2023

Transposition Table.  
Addition of PieceHeatMaps.  
Three-fold repetition.  
Colours & Console design overhaul.  
Incremental Deepening (overhaul of AI multithreading).  
PieceLegalMoves changes.  



## [v6.2] - 07/03/2023

Addition of Settings form.  
Hammad Mode :DD.  
KillerMoves in move ordering.  
Generation Redesign.  



## [v6.1] - 23/12/2022

Improved Opening Book.  
LegalMove Redesign.  
PGN addition to FEN inputter.  



## [v6.0] - 08/12/2022

Val() Perfomance improvements.  
Piece Animations.  
Opening Book.  
Training Modes (Coordinate & Move Practice).  



## [v5.2] - 13/09/2022

Endgame heuristics.  
Minor AI improvements.  
NEA Release - code refactoring and preparation.  



## [b5.2] - 15/08/2022

First Prototype of PieceSquareTable Algorithm (scrapped, then eventually brought back in v7.0).  



## [v5.1] - 12/08/2022

Quiescence Major Fixes.  
Bug Fixes.  
LegalMoveSquare addition.  



## [v5.0] - 04/08/2022

Full AI Structure Redesign (including multithreading for separate depths).  
UI Improvements & Overhaul.  
Prototype Quiescence.  



## [v4.1] - 23/07/2022

Small AI Improvements.  
Last Version Before AI Multithreading redesign.  



## [v4.0] - 11/06/2022

UI Updates.  
Move Ordering.  
Quiescence Prototype.  



## [v3.3] - 30/03/2022

Subtle AI & UI Improvements.  
Last Version with Row Multitasking.  



## [v3.2] - 29/01/2022

AndAlso & OrElse Redesign.  



## [v3.1] - 29/01/2022

UI Overhaul.  
AI Improvements.  



## [v3.0] - 28/01/2022

Addition of Row Multithreading.  



## [v2.0] - 20/01/2022

Addition of Alpha Beta Pruning.  



## [v1.0] - 18/01/2022

Variable Depth.  



## [b1.0] - 11/01/2022

AI with MiniMax implementation (black side only).  
Depth of 1 Search.  



## [b0.2] - 11/12/2021

Check visualisation.  
LegalMove implementation, instead of pseudolegal chess.  
FENExport button.  
Board resetter.  



## [b0.1] - 05/12/2021

First version of my Chess program.  
Pseudo-legal move implementation.  
FEN setter textbox.  
Pseudo-legal move generator.  