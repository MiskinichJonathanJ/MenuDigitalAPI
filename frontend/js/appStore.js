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
        if (!key) {
            return { ...this.#state };
        }

        if (!(key in this.#state)) {
            return undefined;
        }

        return this.#state[key];
    }

    setState(key, value) {
        if (!(key in this.#state)) {
            return;
        }

        const oldValue = this.#state[key];
        if (oldValue === value) {
            return;
        }

        this.#state[key] = value;
        this.#notifyListeners(key, value, oldValue);
    }

    updateState(updates) {
        if (!updates || typeof updates !== 'object') {
            return;
        }

        Object.entries(updates).forEach(([key, value]) => {
            this.setState(key, value);
        });
    }

    subscribe(key, callback) {
        if (typeof callback !== 'function') {
            return () => { };
        }

        if (!(key in this.#state)) {
            return () => { };
        }

        if (!this.#listeners.has(key)) {
            this.#listeners.set(key, new Set());
        }

        this.#listeners.get(key).add(callback);

        return () => {
            const listeners = this.#listeners.get(key);
            if (listeners) {
                listeners.delete(callback);
            }
        };
    }

    #notifyListeners(key, newValue, oldValue) {
        const listeners = this.#listeners.get(key);
        if (!listeners || listeners.size === 0) {
            return;
        }

        listeners.forEach(callback => {
            try {
                callback(newValue, oldValue);
            } catch (error) {
            }
        });
    }

    reset() {
        this.#state = {
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

        Object.keys(this.#state).forEach(key => {
            this.#notifyListeners(key, this.#state[key], undefined);
        });
    }

    getStats() {
        return {
            totalDishes: this.#state.dishes.length,
            totalCategories: this.#state.categories.length,
            cartItems: this.#state.cart.length,
            cartTotal: this.#state.cart.reduce(
                (sum, item) => sum + (item.dish.price * item.quantity),
                0
            ),
            activeListeners: Array.from(this.#listeners.entries()).map(
                ([key, listeners]) => ({ key, count: listeners.size })
            )
        };
    }
}

export const appStore = new AppStore();

export const appState = {
    get dishes() { return appStore.getState('dishes'); },
    set dishes(value) { appStore.setState('dishes', value); },

    get categories() { return appStore.getState('categories'); },
    set categories(value) { appStore.setState('categories', value); },

    get cart() { return appStore.getState('cart'); },
    set cart(value) { appStore.setState('cart', value); },

    get currentCategory() { return appStore.getState('currentCategory'); },
    set currentCategory(value) { appStore.setState('currentCategory', value); },

    get searchTerm() { return appStore.getState('searchTerm'); },
    set searchTerm(value) { appStore.setState('searchTerm', value); },

    get filters() { return appStore.getState('filters'); },
    set filters(value) { appStore.setState('filters', value); }
};