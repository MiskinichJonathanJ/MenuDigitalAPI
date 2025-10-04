class AppStore {
    #state = {
        dishes: [],
        categories: [],
        cart: [],
        currentCategory: 'all',
        searchTerm: '',
        filters: {
            onlyAvailable: false,
            sortByPrice: ''
        }
    };

    #listeners = new Map();

    getState(key) {
        return key ? this.#state[key] : { ...this.#state };
    }

    setState(key, value) {
        if (this.#state[key] === value) return;
        this.#state[key] = value;
        this.#notifyListeners(key, value);
    }

    subscribe(key, callback) {
        if (!this.#listeners.has(key)) {
            this.#listeners.set(key, new Set());
        }
        this.#listeners.get(key).add(callback);
        return () => this.#listeners.get(key)?.delete(callback);
    }
}