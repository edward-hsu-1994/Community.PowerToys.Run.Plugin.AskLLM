# Community.PowerToys.Run.Plugin.AskLLM

This is a plugin for **PowerToys Run** that allows users to interact with Language Model (LLM) services directly from the Power Toys Run launcher. This plugin extends the functionality of Power Toys Run by connecting it with various LLMs hosted at specified URLs. Users have the flexibility to configure different models and prompts through `appsettings.json`, making it versatile for a wide range of use cases such as generating code, answering questions, or seeking explanations related to highlighted or selected text.

The plugin captures the text the user selects within any application and sends it directly to the configured LLM for processing. Users will receive an automated response that provides relevant information or suggested actions based on their input. Unlike traditional API-based solutions, this plugin allows users to interact with LLMs using the models' direct URLs, provided these endpoints support prompts through URL parameters.

<p align="center">
    <img alt="demo" src="https://i.imgur.com/0Qu8g9k.gif">
</p>

> This plugin is inspired by another PowerToys Run plugin [ferraridavide/ChatGPTPowerToys](https://github.com/ferraridavide/ChatGPTPowerToys).

## Prerequisites

- **Power Toys Run**: Ensure you have Power Toys installed and activated.
> Please Update PowerToys to the latest version to use this plugin.
- **LLM Supported Endpoint**: You need access to an LLM endpoint that accepts a prompt via URL parameters.

## Setup Instructions

1. **Installation**:
You can build this project yourself or [download](https://github.com/edward-hsu-1994/Community.PowerToys.Run.Plugin.AskLLM/releases) the compiled version directly. After downloading, copy the compiled result to the `Plugins` folder in the `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins` directory. Then restart PowerToys Run to use this plugin.

2. **Configuration**:
   - Upon installation or upon opening an existing plugin configuration file, the plugin will look for `appsettings.json` in its directory.
   - You can customize the model URL and prompt template in this `appsettings.json` file. The default setup is shown below:
   ```json
   {
       "prompt": "This is my selected text:\r\n```\r\n{selectedText}.\r\n```\r\n{userInput}",
       "prompt_without_selectedText": "{userInput}",
       "url": "https://chatgpt.com/#q={prompt}"
   }
   ```

3. **Using the Plugin**:
    - Select text within any application.(Optional)
    - Press `Alt + Space` to open Power Toys Run.
    - Type `llm` followed by your question, or directly type your question.
    - Press `Enter` to send the query to the LLM endpoint.

## Features

- **Text Selection**: Automatically captures selected text in applications during the execution of a query.
- **Flexible Model Configuration**: Allows users to configure different LLMs through `appsettings.json`.
- **Adjustable Prompt Template**: Users can easily alter the prompt template to accommodate specific tasks or LLM functionalities.

## Support

If you encounter any issues, feel free to raise an issue in this repository.

## Contributing

Contributions to improve and enhance this plugin are welcomed. Please ensure that any changes are well-documented.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

* [ferraridavide/ChatGPTPowerToys](https://github.com/ferraridavide/ChatGPTPowerToys)
