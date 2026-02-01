/**
 * User data transfer object interface
 */
interface UserDto {
    id: string;
    name: string;
}

/**
 * UserListManager - Handles downloading and managing user list data
 */
class UserListManager {
    private apiBaseUrl: string;

    constructor(apiBaseUrl: string = '/api/user') {
        this.apiBaseUrl = apiBaseUrl;
    }

    /**
     * Fetches all users from the API
     */
    async fetchUsers(): Promise<UserDto[]> {
        try {
            const response = await fetch(this.apiBaseUrl, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors', // Enable CORS
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
     * Downloads user list as JSON file
     */
    async downloadAsJson(): Promise<void> {
        try {
            const users = await this.fetchUsers();
            const jsonString = JSON.stringify(users, null, 2);
            const blob = new Blob([jsonString], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            
            const link = document.createElement('a');
            link.href = url;
            link.download = `users-${new Date().toISOString().split('T')[0]}.json`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);

            console.log('User list downloaded successfully as JSON');
        } catch (error) {
            console.error('Error downloading user list:', error);
            alert('Failed to download user list. Please try again.');
        }
    }

    /**
     * Downloads user list as CSV file
     */
    async downloadAsCsv(): Promise<void> {
        try {
            const users = await this.fetchUsers();
            
            // Create CSV header
            const csvHeader = 'ID,Name\n';
            
            // Create CSV rows
            const csvRows = users.map(user => 
                `"${user.id}","${user.name}"`
            ).join('\n');
            
            const csvContent = csvHeader + csvRows;
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            
            const link = document.createElement('a');
            link.href = url;
            link.download = `users-${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);

            console.log('User list downloaded successfully as CSV');
        } catch (error) {
            console.error('Error downloading user list:', error);
            alert('Failed to download user list. Please try again.');
        }
    }

    /**
     * Refreshes the user list display on the page
     */
    async refreshUserList(): Promise<void> {
        try {
            const users = await this.fetchUsers();
            const tableBody = document.querySelector('#userTableBody');
            
            if (!tableBody) {
                console.error('User table body not found');
                return;
            }

            // Clear existing rows
            tableBody.innerHTML = '';

            // Add new rows
            users.forEach(user => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${user.id}</td>
                    <td>${user.name}</td>
                    <td>
                        <a href="/User/Details/${user.id}">Details</a> |
                        <a href="/User/Edit/${user.id}">Edit</a> |
                        <a href="/User/Delete/${user.id}">Delete</a>
                    </td>
                `;
                tableBody.appendChild(row);
            });

            console.log('User list refreshed successfully');
        } catch (error) {
            console.error('Error refreshing user list:', error);
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const userListManager = new UserListManager('https://localhost:7263/api/user');

    // Attach download handlers
    const downloadJsonBtn = document.getElementById('downloadJsonBtn');
    if (downloadJsonBtn) {
        downloadJsonBtn.addEventListener('click', (e) => {
            e.preventDefault();
            userListManager.downloadAsJson();
        });
    }

    const downloadCsvBtn = document.getElementById('downloadCsvBtn');
    if (downloadCsvBtn) {
        downloadCsvBtn.addEventListener('click', (e) => {
            e.preventDefault();
            userListManager.downloadAsCsv();
        });
    }

    const refreshBtn = document.getElementById('refreshListBtn');
    if (refreshBtn) {
        refreshBtn.addEventListener('click', (e) => {
            e.preventDefault();
            userListManager.refreshUserList();
        });
    }
});
