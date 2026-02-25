# Questionnaire Template System

This system allows for dynamic generation of questionnaires from JSON configuration files.

## Structure

- **QuestionnaireManager**: The central controller. Attach this to a GameObject in your scene. 
    - Assign the `jsonFilePath` (relative to StreamingAssets).
    - Assign the `pageContainer` (a Transform with a LayoutGroup is recommended).
    - Map component types (strings from JSON) to Prefabs in `prefabMappings`.
    - Link the `onFinished` UnityEvent to any cleanup or transition logic.
- **QuestionnaireData**: Data models for JSON serialization.
- **QuestionnaireComponent**: Abstract base class for all questionnaire entries.
    - **HeaderComponent**: For displaying section headers (JSON type: "Header").
    - **TextComponent**: For displaying information (JSON type: "TextBlock").
    - **ButtonComponent**: For navigation (Next, Back) or Submission (JSON type: "Buttons"). Supports multiple buttons and displays current page progress (e.g., "1 of 4").
    - **ToggleGroupComponent**: For multiple-choice questions (JSON type: "ToggleGroup").
    - **SliderComponent**: For Likert-scale questions (JSON type: "LikertScale"). Uses standard Unity Slider.
    - **SemanticDifferentialComponent**: Bipolar scale for measuring attitudes between opposites (e.g., Sad to Happy) (JSON type: "SemanticDifferential").
    - **VisualAnalogueComponent**: Continuous scale for sensitive measurements (JSON type: "VisualAnalogue"), returns float values with 2 decimal precision.
    - **VASComponent**: Visual Analogue Scale variants (JSON types: "VAS_Slider", "VAS_Marker"). Supports continuous sliders or discrete marker placement.
    - **DichotomousComponent**: For binary choices (e.g., Yes/No, True/False) (JSON type: "Dichotomous").
    - **RatingScaleComponent**: Discrete numeric ratings (e.g., 1-10) using a row of buttons (JSON type: "RatingScale").
    - **QuestionAnswerComponent**: For questions with a list of answers and checkboxes (JSON type: "QuestionAnswer"). Supports single or multiple selection via `allowMultiple` config.
    - **MatrixLikertComponent**: A grid-style Likert scale with multiple sub-questions (rows) and shared options (columns) (JSON type: "MatrixLikert"). Uses `subQuestions` for rows and `options` for columns.
    - **QuestionDropdownAnswerComponent**: For a single question with multiple options presented in a dropdown (JSON type: "QuestionDropdown").
    - **LikertFacesComponent**: For questions using 3, 5, or 7 face icons as toggles (JSON type: "LikertFaces").

## How to Use

1. **Create Prefabs**:
    - For each component type, create a UI prefab.
    - Attach the corresponding script (e.g., `ToggleGroupComponent`).
    - **Note for ButtonComponent**: This prefab should have a `container` (e.g., with a Horizontal Layout Group), a reference to a `buttonPrefab` (a simple UI Button), and an optional `pageInfoField` (TMP Text) to show page progress.
    - Link required UI references (TMP fields, Buttons, etc.).
2. **Setup Manager**:
    - In your scene, create a Canvas and a container for pages.
    - Add the `QuestionnaireManager` script.
    - Fill the `Prefab Mappings` list with types like "Header", "TextBlock", "Buttons", "ToggleGroup", "LikertScale", "SemanticDifferential", "VisualAnalogue", "VAS_Slider", "VAS_Marker", "Dichotomous", "RatingScale", "QuestionAnswer", "MatrixLikert", "QuestionDropdown", "LikertFaces" and their respective prefabs.
3. **JSON Config**:
    - Create a JSON file in `StreamingAssets`.
    - Use the structure defined in `QuestionnaireSample.json`.
    - **New Fields**: Use `leftLabel` and `rightLabel` for Semantic Differential and VAS components. Use `allowMultiple` (bool) for QuestionAnswer component. Use `subQuestions` (list of strings) for MatrixLikert. Use `minValue` and `maxValue` (int) to define the scale range for Likert, VAS, and Rating components.
    - **Note**: Navigation buttons ("Back", "Next", "Submit") are added automatically to each page and do not need to be defined in JSON (unless using a custom `Buttons` component for non-navigation actions).
4. **Collect Results**:
    - Results are collected automatically when switching pages or submitting.
    - By default, `Submit` logs the results, saves them to `Application.persistentDataPath/results.json`, clears the page, and displays the `thankYouMessage`.

## Extending

To add a new component type:
1. Create a new class inheriting from `QuestionnaireComponent`.
2. Implement `Init` and `GetValue`.
3. Create a prefab with this script.
4. Add it to the `Prefab Mappings` in the `QuestionnaireManager`.
5. Use the new type in your JSON.
