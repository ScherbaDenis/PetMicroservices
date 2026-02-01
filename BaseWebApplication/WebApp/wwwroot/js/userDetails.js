"use strict";
/**
 * UserTemplateManager - Handles fetching and displaying templates for a specific user
 */
class UserTemplateManager {
    constructor(userId, apiBaseUrl = '/proxy/template') {
        this.userId = userId;
        this.apiBaseUrl = apiBaseUrl;
    }
    /**
     * Fetches templates for the current user from the API
     */
    async fetchUserTemplates() {
        try {
            const response = await fetch(`${this.apiBaseUrl}/user/${this.userId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors', // Enable CORS
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const templates = await response.json();
            return templates;
        }
        catch (error) {
            console.error('Error fetching user templates:', error);
            throw error;
        }
    }
    /**
     * Displays templates in the table
     */
    async displayTemplates() {
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
            // Helper function to safely escape HTML
            const escapeHtml = (text) => {
                const div = document.createElement('div');
                div.textContent = text;
                return div.innerHTML;
            };
            // Add template rows with proper XSS protection
            templates.forEach(template => {
                const row = document.createElement('tr');
                const titleCell = document.createElement('td');
                titleCell.textContent = template.title;
                const descriptionCell = document.createElement('td');
                descriptionCell.textContent = template.description || 'N/A';
                const topicCell = document.createElement('td');
                topicCell.textContent = template.topic?.name || 'N/A';
                const tagsCell = document.createElement('td');
                if (template.tags && template.tags.length > 0) {
                    tagsCell.textContent = template.tags.map(tag => tag.name).join(', ');
                }
                else {
                    tagsCell.textContent = 'No tags';
                }
                row.appendChild(titleCell);
                row.appendChild(descriptionCell);
                row.appendChild(topicCell);
                row.appendChild(tagsCell);
                tableBody.appendChild(row);
            });
            console.log('Templates displayed successfully');
        }
        catch (error) {
            console.error('Error displaying templates:', error);
            const tableBody = document.querySelector('#templatesTableBody');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="4" class="text-danger">Failed to load templates. Please try again.</td></tr>';
            }
        }
    }
    /**
     * Downloads user templates as JSON file
     */
    async downloadAsJson() {
        try {
            const templates = await this.fetchUserTemplates();
            const jsonString = JSON.stringify(templates, null, 2);
            const blob = new Blob([jsonString], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `user-${this.userId}-templates-${new Date().toISOString().split('T')[0]}.json`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
            console.log('User templates downloaded successfully as JSON');
        }
        catch (error) {
            console.error('Error downloading user templates:', error);
            alert('Failed to download templates. Please try again.');
        }
    }
    /**
     * Downloads user templates as CSV file
     */
    async downloadAsCsv() {
        try {
            const templates = await this.fetchUserTemplates();
            // Create CSV header
            const csvHeader = 'Title,Description,Topic,Tags\n';
            // Helper function to escape CSV values
            const escapeCsvValue = (value) => {
                // Escape quotes by doubling them and wrap in quotes if contains special chars
                if (value.includes('"') || value.includes(',') || value.includes('\n')) {
                    return `"${value.replace(/"/g, '""')}"`;
                }
                return `"${value}"`;
            };
            // Create CSV rows with proper escaping
            const csvRows = templates.map(template => {
                const title = escapeCsvValue(template.title);
                const description = escapeCsvValue(template.description || 'N/A');
                const topic = escapeCsvValue(template.topic?.name || 'N/A');
                const tags = escapeCsvValue(template.tags && template.tags.length > 0
                    ? template.tags.map(tag => tag.name).join('; ')
                    : 'No tags');
                return `${title},${description},${topic},${tags}`;
            }).join('\n');
            const csvContent = csvHeader + csvRows;
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `user-${this.userId}-templates-${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
            console.log('User templates downloaded successfully as CSV');
        }
        catch (error) {
            console.error('Error downloading user templates:', error);
            alert('Failed to download templates. Please try again.');
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
    // Attach download handlers
    const downloadJsonBtn = document.getElementById('downloadTemplatesJsonBtn');
    if (downloadJsonBtn) {
        downloadJsonBtn.addEventListener('click', (e) => {
            e.preventDefault();
            templateManager.downloadAsJson();
        });
    }
    const downloadCsvBtn = document.getElementById('downloadTemplatesCsvBtn');
    if (downloadCsvBtn) {
        downloadCsvBtn.addEventListener('click', (e) => {
            e.preventDefault();
            templateManager.downloadAsCsv();
        });
    }
    const refreshBtn = document.getElementById('refreshTemplatesBtn');
    if (refreshBtn) {
        refreshBtn.addEventListener('click', (e) => {
            e.preventDefault();
            templateManager.displayTemplates();
        });
    }
});
