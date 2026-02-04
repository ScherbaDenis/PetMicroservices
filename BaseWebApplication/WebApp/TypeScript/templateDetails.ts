/**
 * User data transfer object interface
 */
interface UserDto {
    id: string;
    name: string;
}

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
    usersAccess?: Array<{
        id: string;
        name: string;
    }>;
}

/**
 * TemplateUserManager - Handles fetching and displaying users for a specific template
 */
class TemplateUserManager {
    private templateId: string;

    constructor(templateId: string) {
        this.templateId = templateId;
    }

    /**
     * Fetches template details including users with access
     * Calls: GET /proxy/template/{templateId}
     */
    async fetchTemplate(): Promise<TemplateDto> {
        try {
            const response = await fetch(`/proxy/template/${this.templateId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const template: TemplateDto = await response.json();
            return template;
        } catch (error) {
            console.error('Error fetching template:', error);
            throw error;
        }
    }

    /**
     * Fetches all available users from the Template API
     * Calls: GET /proxy/user
     */
    async fetchAllUsers(): Promise<UserDto[]> {
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
            console.error('Error fetching all users:', error);
            throw error;
        }
    }

    /**
     * Assigns a user to the template
     * Calls: POST /proxy/template/{templateId}/assign/{userId}
     */
    async assignUser(userId: string): Promise<void> {
        try {
            const response = await fetch(`/proxy/template/${this.templateId}/assign/${userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            console.log('User assigned successfully');
        } catch (error) {
            console.error('Error assigning user:', error);
            throw error;
        }
    }

    /**
     * Unassigns a user from the template
     * Calls: DELETE /proxy/template/{templateId}/assign/{userId}
     */
    async unassignUser(userId: string): Promise<void> {
        try {
            const response = await fetch(`/proxy/template/${this.templateId}/assign/${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            console.log('User unassigned successfully');
        } catch (error) {
            console.error('Error unassigning user:', error);
            throw error;
        }
    }

    /**
     * Displays users with access to the template
     */
    async displayUsersAccess(): Promise<void> {
        try {
            const template = await this.fetchTemplate();
            const users = template.usersAccess || [];
            const tableBody = document.querySelector('#usersAccessTableBody');
            const noUsersAccessMessage = document.querySelector('#noUsersAccessMessage');
            const usersAccessSection = document.querySelector('#usersAccessSection');
            
            if (!tableBody || !usersAccessSection) {
                console.error('Users access table elements not found');
                return;
            }

            // Clear existing rows
            tableBody.innerHTML = '';

            if (users.length === 0) {
                if (noUsersAccessMessage) {
                    noUsersAccessMessage.classList.remove('d-none');
                }
                return;
            }

            if (noUsersAccessMessage) {
                noUsersAccessMessage.classList.add('d-none');
            }

            // Add user rows with proper XSS protection
            users.forEach(user => {
                const row = document.createElement('tr');
                
                const nameCell = document.createElement('td');
                nameCell.textContent = user.name;
                
                const actionsCell = document.createElement('td');
                const unassignBtn = document.createElement('button');
                unassignBtn.className = 'btn btn-sm btn-danger';
                unassignBtn.textContent = 'Unassign';
                unassignBtn.onclick = async () => {
                    await this.handleUnassign(user.id);
                };
                actionsCell.appendChild(unassignBtn);
                
                row.appendChild(nameCell);
                row.appendChild(actionsCell);
                tableBody.appendChild(row);
            });

            console.log('Users with access displayed successfully');
        } catch (error) {
            console.error('Error displaying users with access:', error);
            const tableBody = document.querySelector('#usersAccessTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="2" class="text-danger">Failed to load users. Please try again.</td></tr>';
            }
        }
    }

    /**
     * Displays available users to assign to the template
     */
    async displayAvailableUsers(): Promise<void> {
        try {
            const [allUsers, template] = await Promise.all([
                this.fetchAllUsers(),
                this.fetchTemplate()
            ]);

            const usersWithAccess = template.usersAccess || [];
            const usersWithAccessIds = new Set(usersWithAccess.map(u => u.id));
            const availableUsers = allUsers.filter(u => !usersWithAccessIds.has(u.id));

            const tableBody = document.querySelector('#availableUsersTableBody');
            const noAvailableUsersMessage = document.querySelector('#noAvailableUsersMessage');
            const availableUsersSection = document.querySelector('#availableUsersSection');
            
            if (!tableBody || !availableUsersSection) {
                console.error('Available users table elements not found');
                return;
            }

            // Clear existing rows
            tableBody.innerHTML = '';

            if (availableUsers.length === 0) {
                if (noAvailableUsersMessage) {
                    noAvailableUsersMessage.classList.remove('d-none');
                }
                return;
            }

            if (noAvailableUsersMessage) {
                noAvailableUsersMessage.classList.add('d-none');
            }

            // Add user rows with proper XSS protection
            availableUsers.forEach(user => {
                const row = document.createElement('tr');
                
                const nameCell = document.createElement('td');
                nameCell.textContent = user.name;
                
                const actionsCell = document.createElement('td');
                const assignBtn = document.createElement('button');
                assignBtn.className = 'btn btn-sm btn-success';
                assignBtn.textContent = 'Assign';
                assignBtn.onclick = async () => {
                    await this.handleAssign(user.id);
                };
                actionsCell.appendChild(assignBtn);
                
                row.appendChild(nameCell);
                row.appendChild(actionsCell);
                tableBody.appendChild(row);
            });

            console.log('Available users displayed successfully');
        } catch (error) {
            console.error('Error displaying available users:', error);
            const tableBody = document.querySelector('#availableUsersTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="2" class="text-danger">Failed to load available users. Please try again.</td></tr>';
            }
        }
    }

    /**
     * Handles assigning a user to the template
     */
    async handleAssign(userId: string): Promise<void> {
        try {
            await this.assignUser(userId);
            // Refresh both tables
            await this.displayUsersAccess();
            await this.displayAvailableUsers();
            alert('User assigned successfully!');
        } catch (error) {
            console.error('Error in handleAssign:', error);
            alert('Failed to assign user. Please try again.');
        }
    }

    /**
     * Handles unassigning a user from the template
     */
    async handleUnassign(userId: string): Promise<void> {
        try {
            await this.unassignUser(userId);
            // Refresh both tables
            await this.displayUsersAccess();
            await this.displayAvailableUsers();
            alert('User unassigned successfully!');
        } catch (error) {
            console.error('Error in handleUnassign:', error);
            alert('Failed to unassign user. Please try again.');
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const templateIdElement = document.getElementById('templateId');
    
    if (!templateIdElement) {
        console.error('Template ID element not found');
        return;
    }
    
    const templateId = templateIdElement.getAttribute('data-template-id');
    
    if (!templateId) {
        console.error('Template ID not provided');
        return;
    }

    const templateUserManager = new TemplateUserManager(templateId);

    // Load both users with access and available users on page load
    templateUserManager.displayUsersAccess();
    templateUserManager.displayAvailableUsers();
});
