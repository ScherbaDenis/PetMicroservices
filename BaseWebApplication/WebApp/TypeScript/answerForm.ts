/**
 * User data transfer object interface
 */
interface UserDto {
    id: string;
    name: string;
}

/**
 * Question data transfer object interface
 */
interface QuestionDto {
    id: string;
    title: string;
}

/**
 * Template data transfer object interface
 */
interface TemplateDto {
    id: string;
    title: string;
}

/**
 * AnswerFormManager - Handles dynamic loading of dropdown data for Answer Create/Edit forms
 */
class AnswerFormManager {
    private userSelectElement: HTMLSelectElement | null;
    private questionSelectElement: HTMLSelectElement | null;
    private templateSelectElement: HTMLSelectElement | null;
    private currentUserId: string | null = null;
    private currentQuestionId: string | null = null;
    private currentTemplateId: string | null = null;

    constructor() {
        this.userSelectElement = document.getElementById('UserId') as HTMLSelectElement;
        this.questionSelectElement = document.getElementById('QuestionId') as HTMLSelectElement;
        this.templateSelectElement = document.getElementById('TemplateId') as HTMLSelectElement;
        
        // Get current values from the select elements (for Edit mode)
        if (this.userSelectElement && this.userSelectElement.value) {
            this.currentUserId = this.userSelectElement.value;
        }
        if (this.questionSelectElement && this.questionSelectElement.value) {
            this.currentQuestionId = this.questionSelectElement.value;
        }
        if (this.templateSelectElement && this.templateSelectElement.value) {
            this.currentTemplateId = this.templateSelectElement.value;
        }
    }

    /**
     * Fetches all users from the API
     * Calls: GET /proxy/user
     */
    async fetchUsers(): Promise<UserDto[]> {
        try {
            const response = await fetch('/proxy/user', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const users: UserDto[] = await response.json();
            return users;
        } catch (error) {
            console.error('Error fetching users:', error);
            throw error;
        }
    }

    /**
     * Fetches all questions from the API
     * Calls: GET /proxy/template/question
     */
    async fetchQuestions(): Promise<QuestionDto[]> {
        try {
            const response = await fetch('/proxy/template/question', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const questions: QuestionDto[] = await response.json();
            return questions;
        } catch (error) {
            console.error('Error fetching questions:', error);
            throw error;
        }
    }

    /**
     * Fetches all templates from the API
     * Calls: GET /proxy/template
     */
    async fetchTemplates(): Promise<TemplateDto[]> {
        try {
            const response = await fetch('/proxy/template', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const templates: TemplateDto[] = await response.json();
            return templates;
        } catch (error) {
            console.error('Error fetching templates:', error);
            throw error;
        }
    }

    /**
     * Populates the user dropdown
     */
    async populateUserDropdown(): Promise<void> {
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
                
                this.userSelectElement!.appendChild(option);
            });

            console.log('User dropdown populated successfully with', users.length, 'users');
        } catch (error) {
            console.error('Error populating user dropdown:', error);
            if (this.userSelectElement) {
                this.userSelectElement.innerHTML = '<option value="">Error loading users</option>';
            }
        }
    }

    /**
     * Populates the question dropdown
     */
    async populateQuestionDropdown(): Promise<void> {
        if (!this.questionSelectElement) {
            console.error('Question select element not found');
            return;
        }

        try {
            const questions = await this.fetchQuestions();
            
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
                
                this.questionSelectElement!.appendChild(option);
            });

            console.log('Question dropdown populated successfully with', questions.length, 'questions');
        } catch (error) {
            console.error('Error populating question dropdown:', error);
            if (this.questionSelectElement) {
                this.questionSelectElement.innerHTML = '<option value="">Error loading questions</option>';
            }
        }
    }

    /**
     * Populates the template dropdown
     */
    async populateTemplateDropdown(): Promise<void> {
        if (!this.templateSelectElement) {
            console.error('Template select element not found');
            return;
        }

        try {
            const templates = await this.fetchTemplates();
            
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
                
                this.templateSelectElement!.appendChild(option);
            });

            console.log('Template dropdown populated successfully with', templates.length, 'templates');
        } catch (error) {
            console.error('Error populating template dropdown:', error);
            if (this.templateSelectElement) {
                this.templateSelectElement.innerHTML = '<option value="">Error loading templates</option>';
            }
        }
    }

    /**
     * Initializes all dropdowns
     */
    async initialize(): Promise<void> {
        await Promise.all([
            this.populateUserDropdown(),
            this.populateQuestionDropdown(),
            this.populateTemplateDropdown()
        ]);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const formManager = new AnswerFormManager();
    formManager.initialize();
});
