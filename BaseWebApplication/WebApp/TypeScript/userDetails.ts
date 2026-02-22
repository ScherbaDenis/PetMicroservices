import { buildApiUrl } from './apiConfig';

/**
 * Template data transfer object interface
 */
interface TemplateDto {
    id: string;
    title: string;
    description?: string;
    owner?: {
        id: string;
        name: string;
    };
    topic?: {
        id: string;
        name: string;
    };
    tags?: Array<{
        id: string;
        name: string;
    }>;
}

/**
 * UserTemplateManager - Handles fetching and displaying templates for a specific user
 */
class UserTemplateManager {
    private userId: string;

    constructor(userId: string) {
        this.userId = userId;
    }

    /**
     * Fetches templates for the current user from the Template API
     * Calls: GET {API_GATEWAY_URL}/template/user/{userId}
     * Direct call to API Gateway
     */
    async fetchUserTemplates(): Promise<TemplateDto[]> {
        try {
            const response = await fetch(buildApiUrl(`/template/user/${this.userId}`), {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const templates: TemplateDto[] = await response.json();
            return templates;
        } catch (error) {
            console.error('Error fetching user templates:', error);
            throw error;
        }
    }

    /**
     * Displays templates in the table
     */
    async displayTemplates(): Promise<void> {
        try {
            const templates = await this.fetchUserTemplates();
            const tableBody = document.querySelector('#templatesTableBody');
            const noTemplatesMessage = document.querySelector('#noTemplatesMessage');
            const templatesSection = document.querySelector('#templatesSection');
            
            if (!tableBody || !templatesSection) {
                console.error('Templates table elements not found');
                return;
            }

            // Clear existing rows
            tableBody.innerHTML = '';

            if (templates.length === 0) {
                if (noTemplatesMessage) {
                    noTemplatesMessage.classList.remove('d-none');
                }
                return;
            }

            if (noTemplatesMessage) {
                noTemplatesMessage.classList.add('d-none');
            }

            // Add template rows with proper XSS protection
            templates.forEach(template => {
                const row = document.createElement('tr');
                
                const titleCell = document.createElement('td');
                titleCell.textContent = template.title;
                
                const descriptionCell = document.createElement('td');
                descriptionCell.textContent = template.description || 'N/A';
                
                const topicCell = document.createElement('td');
                topicCell.textContent = (template.topic && template.topic.name) ? template.topic.name : 'N/A';
                
                const tagsCell = document.createElement('td');
                if (template.tags && template.tags.length > 0) {
                    tagsCell.textContent = template.tags.map(tag => tag.name).join(', ');
                } else {
                    tagsCell.textContent = 'No tags';
                }
                
                row.appendChild(titleCell);
                row.appendChild(descriptionCell);
                row.appendChild(topicCell);
                row.appendChild(tagsCell);
                tableBody.appendChild(row);
            });

            console.log('Templates displayed successfully');
        } catch (error) {
            console.error('Error displaying templates:', error);
            const tableBody = document.querySelector('#templatesTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="4" class="text-danger">Failed to load templates. Please try again.</td></tr>';
            }
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const userIdElement = document.getElementById('userId');
    
    if (!userIdElement) {
        console.error('User ID element not found');
        return;
    }
    
    const userId = userIdElement.getAttribute('data-user-id');
    
    if (!userId) {
        console.error('User ID not provided');
        return;
    }

    const templateManager = new UserTemplateManager(userId);

    // Load templates on page load
    templateManager.displayTemplates();
});
