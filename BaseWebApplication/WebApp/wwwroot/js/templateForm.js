"use strict";
/**
 * TemplateFormManager - Handles dynamic loading of dropdown data for Template Create/Edit forms
 */
class TemplateFormManager {
    constructor() {
        this.currentOwnerId = null;
        this.currentTopicId = null;
        this.ownerSelectElement = document.getElementById('Owner_Id');
        this.topicSelectElement = document.getElementById('Topic_Id');
        // Get current values from the select elements (for Edit mode)
        if (this.ownerSelectElement && this.ownerSelectElement.value) {
            this.currentOwnerId = this.ownerSelectElement.value;
        }
        if (this.topicSelectElement && this.topicSelectElement.value) {
            this.currentTopicId = parseInt(this.topicSelectElement.value);
        }
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
                mode: 'cors',
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
     * Fetches all topics from the API
     * Calls: GET /proxy/topic
     */
    async fetchTopics() {
        try {
            const response = await fetch('/proxy/topic', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                mode: 'cors',
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const topics = await response.json();
            return topics;
        }
        catch (error) {
            console.error('Error fetching topics:', error);
            throw error;
        }
    }
    /**
     * Populates the owner dropdown with users
     */
    async populateOwnerDropdown() {
        if (!this.ownerSelectElement) {
            console.error('Owner select element not found');
            return;
        }
        try {
            const users = await this.fetchUsers();
            // Clear existing options except the first one (placeholder if exists)
            while (this.ownerSelectElement.options.length > 0) {
                this.ownerSelectElement.remove(0);
            }
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select Owner --';
            this.ownerSelectElement.appendChild(emptyOption);
            // Add user options
            users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.id;
                option.textContent = user.name;
                // Select the current owner if in Edit mode
                if (this.currentOwnerId && user.id === this.currentOwnerId) {
                    option.selected = true;
                }
                this.ownerSelectElement.appendChild(option);
            });
            console.log('Owner dropdown populated successfully with', users.length, 'users');
        }
        catch (error) {
            console.error('Error populating owner dropdown:', error);
            if (this.ownerSelectElement) {
                this.ownerSelectElement.innerHTML = '<option value="">Error loading owners</option>';
            }
        }
    }
    /**
     * Populates the topic dropdown with topics
     */
    async populateTopicDropdown() {
        if (!this.topicSelectElement) {
            console.error('Topic select element not found');
            return;
        }
        try {
            const topics = await this.fetchTopics();
            // Clear existing options
            while (this.topicSelectElement.options.length > 0) {
                this.topicSelectElement.remove(0);
            }
            // Add empty option
            const emptyOption = document.createElement('option');
            emptyOption.value = '';
            emptyOption.textContent = '-- Select Topic --';
            this.topicSelectElement.appendChild(emptyOption);
            // Add topic options
            topics.forEach(topic => {
                const option = document.createElement('option');
                option.value = topic.id.toString();
                option.textContent = topic.name;
                // Select the current topic if in Edit mode
                if (this.currentTopicId && topic.id === this.currentTopicId) {
                    option.selected = true;
                }
                this.topicSelectElement.appendChild(option);
            });
            console.log('Topic dropdown populated successfully with', topics.length, 'topics');
        }
        catch (error) {
            console.error('Error populating topic dropdown:', error);
            if (this.topicSelectElement) {
                this.topicSelectElement.innerHTML = '<option value="">Error loading topics</option>';
            }
        }
    }
    /**
     * Initializes both dropdowns
     */
    async initialize() {
        await Promise.all([
            this.populateOwnerDropdown(),
            this.populateTopicDropdown()
        ]);
    }
}
// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const formManager = new TemplateFormManager();
    formManager.initialize();
});
