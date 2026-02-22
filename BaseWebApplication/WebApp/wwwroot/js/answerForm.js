"use strict";
/**
 * AnswerFormManager - Handles dynamic loading of dropdown data for Answer Create/Edit forms
 * with cascading dropdown logic
 */
class AnswerFormManager {
    constructor() {
        this.currentUserId = null;
        this.currentTemplateId = null;
        this.currentQuestionId = null;
        this.templates = [];
        this.questions = [];
        this.userSelectElement = document.getElementById('UserId');
        this.templateSelectElement = document.getElementById('TemplateId');
        this.questionSelectElement = document.getElementById('QuestionId');
        this.answerContainerElement = document.getElementById('answerContainer');
        this.answerInputElement = document.getElementById('AnswerValue');
        // Get current values from the select elements (for Edit mode)
        if (this.userSelectElement && this.userSelectElement.value) {
            this.currentUserId = this.userSelectElement.value;
        }
        if (this.templateSelectElement && this.templateSelectElement.value) {
            this.currentTemplateId = this.templateSelectElement.value;
        }
        if (this.questionSelectElement && this.questionSelectElement.value) {
            this.currentQuestionId = this.questionSelectElement.value;
        }
        // Setup event listeners for cascading dropdowns
        this.setupEventListeners();
    }
    /**
     * Fetches all users from the API
     * Calls: GET /proxy/user
     */
    async fetchUsers() {
        try {
            const response = await fetch('/proxy/user', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const users = await response.json();
            return users;
        }
        catch (error) {
            console.error('Error fetching users:', error);
            throw error;
        }
    }
    /**
     * Fetches all questions from the API
     * Calls: GET /proxy/question
     */
    async fetchQuestions() {
        try {
            const response = await fetch('/proxy/question', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const questions = await response.json();
            return questions;
        }
        catch (error) {
            console.error('Error fetching questions:', error);
            throw error;
        }
    }
    /**
     * Fetches all templates from the API
     * Calls: GET /proxy/template
     */
    async fetchTemplates() {
        try {
            const response = await fetch('/proxy/template', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const templates = await response.json();
            return templates;
        }
        catch (error) {
            console.error('Error fetching templates:', error);
            throw error;
        }
    }
    /**
     * Fetches templates for a specific user
     * Calls: GET /proxy/template/user/{userId}
     */
    async fetchTemplatesByUserId(userId) {
        try {
            const response = await fetch(`/proxy/template/user/${userId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const templates = await response.json();
            return templates;
        }
        catch (error) {
            console.error('Error fetching templates by user:', error);
            throw error;
        }
    }
    /**
     * Setup event listeners for cascading dropdowns
     */
    setupEventListeners() {
        // User selection change
        if (this.userSelectElement) {
            this.userSelectElement.addEventListener('change', async (e) => {
                const userId = e.target.value;
                if (userId) {
                    await this.populateTemplateDropdownForUser(userId);
                }
                else {
                    this.clearTemplateDropdown();
                }
                // Clear dependent dropdowns
                this.clearQuestionDropdown();
                this.hideAnswerInput();
            });
        }
        // Template selection change
        if (this.templateSelectElement) {
            this.templateSelectElement.addEventListener('change', async (e) => {
                const templateId = e.target.value;
                if (templateId) {
                    await this.populateQuestionDropdownForTemplate(templateId);
                }
                else {
                    this.clearQuestionDropdown();
                }
                // Clear dependent dropdown
                this.hideAnswerInput();
            });
        }
        // Question selection change
        if (this.questionSelectElement) {
            this.questionSelectElement.addEventListener('change', (e) => {
                const questionId = e.target.value;
                if (questionId) {
                    this.showAnswerInputForQuestion(questionId);
                }
                else {
                    this.hideAnswerInput();
                }
            });
        }
    }
    /**
     * Clears the template dropdown
     */
    clearTemplateDropdown() {
        if (this.templateSelectElement) {
            while (this.templateSelectElement.options.length > 0) {
                this.templateSelectElement.remove(0);
            }
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select User First --';
            this.templateSelectElement.appendChild(emptyOption);
        }
    }
    /**
     * Clears the question dropdown
     */
    clearQuestionDropdown() {
        if (this.questionSelectElement) {
            while (this.questionSelectElement.options.length > 0) {
                this.questionSelectElement.remove(0);
            }
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select Template First --';
            this.questionSelectElement.appendChild(emptyOption);
        }
    }
    /**
     * Hides the answer input container
     */
    hideAnswerInput() {
        if (this.answerContainerElement) {
            this.answerContainerElement.style.display = 'none';
        }
    }
    /**
     * Maps QuestionType to AnswerType enum value
     */
    mapQuestionTypeToAnswerType(questionType) {
        switch (questionType) {
            case 'SingleLineString':
                return 0; // SingleLineString
            case 'MultiLineText':
                return 1; // MultiLineText
            case 'PositiveInteger':
                return 2; // PositiveInteger
            case 'Checkbox':
                return 3; // Checkbox
            case 'Boolean':
                return 4; // Boolean
            default:
                return 0; // Default to SingleLineString
        }
    }
    /**
     * Shows and configures the answer input based on question type
     */
    showAnswerInputForQuestion(questionId) {
        const question = this.questions.find(q => q.id === questionId);
        if (!question || !this.answerContainerElement) {
            return;
        }
        const questionType = question.questionType || '';
        // Set the hidden AnswerType field based on QuestionType
        const answerTypeInput = document.getElementById('AnswerType');
        if (answerTypeInput) {
            answerTypeInput.value = this.mapQuestionTypeToAnswerType(questionType).toString();
        }
        let inputHtml = '';
        switch (questionType) {
            case 'SingleLineString':
                inputHtml = '<input type="text" id="AnswerValue" name="AnswerValue" class="form-control" />';
                break;
            case 'MultiLineText':
                inputHtml = '<textarea id="AnswerValue" name="AnswerValue" class="form-control" rows="4"></textarea>';
                break;
            case 'PositiveInteger':
                inputHtml = '<input type="number" id="AnswerValue" name="AnswerValue" class="form-control" min="0" step="1" />';
                break;
            case 'Checkbox':
                if (question.options && question.options.length > 0) {
                    inputHtml = '<div class="checkbox-group">';
                    question.options.forEach((option, index) => {
                        inputHtml += `
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="option_${index}" name="AnswerOptions" value="${option}" />
                                <label class="form-check-label" for="option_${index}">${option}</label>
                            </div>`;
                    });
                    inputHtml += '</div>';
                    inputHtml += '<input type="hidden" id="AnswerValue" name="AnswerValue" />';
                }
                else {
                    inputHtml = '<p class="text-warning">No options available for this checkbox question.</p>';
                }
                break;
            case 'Boolean':
                inputHtml = `
                    <div class="form-check">
                        <input type="radio" class="form-check-input" id="answer_true" name="AnswerValue" value="true" />
                        <label class="form-check-label" for="answer_true">Yes</label>
                    </div>
                    <div class="form-check">
                        <input type="radio" class="form-check-input" id="answer_false" name="AnswerValue" value="false" />
                        <label class="form-check-label" for="answer_false">No</label>
                    </div>`;
                break;
            default:
                inputHtml = '<textarea id="AnswerValue" name="AnswerValue" class="form-control" rows="4"></textarea>';
        }
        // Find the input container within answerContainerElement
        const inputContainer = this.answerContainerElement.querySelector('.answer-input-container');
        if (inputContainer) {
            inputContainer.innerHTML = inputHtml;
            // Setup checkbox change handler if needed
            if (questionType === 'Checkbox') {
                const checkboxes = inputContainer.querySelectorAll('input[name="AnswerOptions"]');
                const hiddenInput = inputContainer.querySelector('#AnswerValue');
                checkboxes.forEach(cb => {
                    cb.addEventListener('change', () => {
                        const selected = Array.from(checkboxes)
                            .filter(c => c.checked)
                            .map(c => c.value);
                        if (hiddenInput) {
                            hiddenInput.value = JSON.stringify(selected);
                        }
                    });
                });
            }
        }
        this.answerContainerElement.style.display = 'block';
    }
    /**
     * Populates the user dropdown
     */
    async populateUserDropdown() {
        if (!this.userSelectElement) {
            console.error('User select element not found');
            return;
        }
        try {
            const users = await this.fetchUsers();
            // Clear existing options
            while (this.userSelectElement.options.length > 0) {
                this.userSelectElement.remove(0);
            }
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select User --';
            this.userSelectElement.appendChild(emptyOption);
            // Add user options
            users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.id;
                option.textContent = user.name;
                // Select the current user if in Edit mode
                if (this.currentUserId && user.id === this.currentUserId) {
                    option.selected = true;
                }
                this.userSelectElement.appendChild(option);
            });
            console.log('User dropdown populated successfully with', users.length, 'users');
        }
        catch (error) {
            console.error('Error populating user dropdown:', error);
            if (this.userSelectElement) {
                this.userSelectElement.innerHTML = '<option value="">Error loading users</option>';
            }
        }
    }
    /**
     * Populates the template dropdown for a specific user
     */
    async populateTemplateDropdownForUser(userId) {
        if (!this.templateSelectElement) {
            console.error('Template select element not found');
            return;
        }
        try {
            const templates = await this.fetchTemplatesByUserId(userId);
            this.templates = templates; // Store for later use
            // Clear existing options
            while (this.templateSelectElement.options.length > 0) {
                this.templateSelectElement.remove(0);
            }
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select Template --';
            this.templateSelectElement.appendChild(emptyOption);
            // Add template options
            templates.forEach(template => {
                const option = document.createElement('option');
                option.value = template.id;
                option.textContent = template.title;
                // Select the current template if in Edit mode
                if (this.currentTemplateId && template.id === this.currentTemplateId) {
                    option.selected = true;
                }
                this.templateSelectElement.appendChild(option);
            });
            console.log('Template dropdown populated successfully with', templates.length, 'templates for user', userId);
        }
        catch (error) {
            console.error('Error populating template dropdown for user:', error);
            if (this.templateSelectElement) {
                this.templateSelectElement.innerHTML = '<option value="">Error loading templates</option>';
            }
        }
    }
    /**
     * Populates the question dropdown for a specific template
     */
    async populateQuestionDropdownForTemplate(templateId) {
        if (!this.questionSelectElement) {
            console.error('Question select element not found');
            return;
        }
        try {
            // Get the template to extract its questions
            const response = await fetch(`/proxy/template/${templateId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const template = await response.json();
            const questions = template.questions || [];
            this.questions = questions; // Store for later use
            // Clear existing options
            while (this.questionSelectElement.options.length > 0) {
                this.questionSelectElement.remove(0);
            }
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select Question --';
            this.questionSelectElement.appendChild(emptyOption);
            // Add question options
            questions.forEach(question => {
                const option = document.createElement('option');
                option.value = question.id;
                option.textContent = question.title;
                // Select the current question if in Edit mode
                if (this.currentQuestionId && question.id === this.currentQuestionId) {
                    option.selected = true;
                }
                this.questionSelectElement.appendChild(option);
            });
            console.log('Question dropdown populated successfully with', questions.length, 'questions for template', templateId);
        }
        catch (error) {
            console.error('Error populating question dropdown for template:', error);
            if (this.questionSelectElement) {
                this.questionSelectElement.innerHTML = '<option value="">Error loading questions</option>';
            }
        }
    }
    /**
     * Initializes all dropdowns
     */
    async initialize() {
        // Always populate users
        await this.populateUserDropdown();
        // If we have a current user (Edit mode), populate templates for that user
        if (this.currentUserId) {
            await this.populateTemplateDropdownForUser(this.currentUserId);
            // If we have a current template (Edit mode), populate questions for that template
            if (this.currentTemplateId) {
                await this.populateQuestionDropdownForTemplate(this.currentTemplateId);
                // If we have a current question (Edit mode), show the answer input
                if (this.currentQuestionId) {
                    this.showAnswerInputForQuestion(this.currentQuestionId);
                }
            }
        }
        else {
            // Create mode: start with empty templates and questions
            this.clearTemplateDropdown();
            this.clearQuestionDropdown();
            this.hideAnswerInput();
        }
    }
}
// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const formManager = new AnswerFormManager();
    formManager.initialize();
});
