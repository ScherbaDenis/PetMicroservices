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
     * Calls: GET /proxy/template/user/{userId}
     * Direct call to Template microservice via YARP proxy
     */
    async fetchUserTemplates(): Promise<TemplateDto[]> {
        try {
            const response = await fetch(`/proxy/template/user/${this.userId}`, {
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
            console.error('Error fetching user templates:', error);
            throw error;
        }
    }

    /**
     * Fetches all available templates from the Template API
     * Calls: GET /proxy/template
     */
    async fetchAllTemplates(): Promise<TemplateDto[]> {
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
            console.error('Error fetching all templates:', error);
            throw error;
        }
    }

    /**
     * Assigns a template to the user
     * Calls: POST /proxy/template/{templateId}/assign/{userId}
     */
    async assignTemplate(templateId: string): Promise<void> {
        try {
            const response = await fetch(`/proxy/template/${templateId}/assign/${this.userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            console.log('Template assigned successfully');
        } catch (error) {
            console.error('Error assigning template:', error);
            throw error;
        }
    }

    /**
     * Unassigns a template from the user
     * Calls: DELETE /proxy/template/{templateId}/assign/{userId}
     */
    async unassignTemplate(templateId: string): Promise<void> {
        try {
            const response = await fetch(`/proxy/template/${templateId}/assign/${this.userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            console.log('Template unassigned successfully');
        } catch (error) {
            console.error('Error unassigning template:', error);
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
                
                const actionsCell = document.createElement('td');
                const unassignBtn = document.createElement('button');
                unassignBtn.className = 'btn btn-sm btn-danger';
                unassignBtn.textContent = 'Unassign';
                unassignBtn.onclick = async () => {
                    await this.handleUnassign(template.id);
                };
                actionsCell.appendChild(unassignBtn);
                
                row.appendChild(titleCell);
                row.appendChild(descriptionCell);
                row.appendChild(topicCell);
                row.appendChild(tagsCell);
                row.appendChild(actionsCell);
                tableBody.appendChild(row);
            });

            console.log('Templates displayed successfully');
        } catch (error) {
            console.error('Error displaying templates:', error);
            const tableBody = document.querySelector('#templatesTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-danger">Failed to load templates. Please try again.</td></tr>';
            }
        }
    }

    /**
     * Displays available templates to assign in the table
     */
    async displayAvailableTemplates(): Promise<void> {
        try {
            const [allTemplates, userTemplates] = await Promise.all([
                this.fetchAllTemplates(),
                this.fetchUserTemplates()
            ]);

            const userTemplateIds = new Set(userTemplates.map(t => t.id));
            const availableTemplates = allTemplates.filter(t => !userTemplateIds.has(t.id));

            const tableBody = document.querySelector('#availableTemplatesTableBody');
            const noAvailableTemplatesMessage = document.querySelector('#noAvailableTemplatesMessage');
            const availableTemplatesSection = document.querySelector('#availableTemplatesSection');
            
            if (!tableBody || !availableTemplatesSection) {
                console.error('Available templates table elements not found');
                return;
            }

            // Clear existing rows
            tableBody.innerHTML = '';

            if (availableTemplates.length === 0) {
                if (noAvailableTemplatesMessage) {
                    noAvailableTemplatesMessage.classList.remove('d-none');
                }
                return;
            }

            if (noAvailableTemplatesMessage) {
                noAvailableTemplatesMessage.classList.add('d-none');
            }

            // Add template rows with proper XSS protection
            availableTemplates.forEach(template => {
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
                
                const actionsCell = document.createElement('td');
                const assignBtn = document.createElement('button');
                assignBtn.className = 'btn btn-sm btn-success';
                assignBtn.textContent = 'Assign';
                assignBtn.onclick = async () => {
                    await this.handleAssign(template.id);
                };
                actionsCell.appendChild(assignBtn);
                
                row.appendChild(titleCell);
                row.appendChild(descriptionCell);
                row.appendChild(topicCell);
                row.appendChild(tagsCell);
                row.appendChild(actionsCell);
                tableBody.appendChild(row);
            });

            console.log('Available templates displayed successfully');
        } catch (error) {
            console.error('Error displaying available templates:', error);
            const tableBody = document.querySelector('#availableTemplatesTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-danger">Failed to load available templates. Please try again.</td></tr>';
            }
        }
    }

    /**
     * Handles assigning a template to the user
     */
    async handleAssign(templateId: string): Promise<void> {
        try {
            await this.assignTemplate(templateId);
            // Refresh both tables
            await this.displayTemplates();
            await this.displayAvailableTemplates();
            alert('Template assigned successfully!');
        } catch (error) {
            console.error('Error in handleAssign:', error);
            alert('Failed to assign template. Please try again.');
        }
    }

    /**
     * Handles unassigning a template from the user
     */
    async handleUnassign(templateId: string): Promise<void> {
        try {
            await this.unassignTemplate(templateId);
            // Refresh both tables
            await this.displayTemplates();
            await this.displayAvailableTemplates();
            alert('Template unassigned successfully!');
        } catch (error) {
            console.error('Error in handleUnassign:', error);
            alert('Failed to unassign template. Please try again.');
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

    // Load both user templates and available templates on page load
    templateManager.displayTemplates();
    templateManager.displayAvailableTemplates();
});
