const api = {
  client: {
    order: {
      getAll: 'api/client/order',
      get: 'api/client/order/{id}',
      create: 'api/client/order',
      cancel: 'api/client/order/{id}/cancel',
    },
  },
  business: {
    inventory: {
      delete: 'api/business/inventory-record/{id}',
      create: 'api/business/inventory-record/product/{productId}',
    },
    product: {
      create: 'api/business/product',
      update: 'api/business/product/{id}',
      delete: 'api/business/product/{id}',
    },
    store: {
      getAll: 'api/business/store',
      create: 'api/business/store',
    }
  },
  public: {
    identity: {
      userInfo: '/api/identity/user-info',
      login: '/api/identity/login',
      logout: '/api/identity/logout',
      register: 'api/identity/register',
      updateUser: 'api/identity/update-user',
      becomeSeller: 'api/identity/become-seller',
    },
    inventory: {
      get: 'api/public/inventory-record/{id}',
    },
    store: {
      getAll: 'api/public/store',
      get: 'api/public/store/{id}',
    },
    product: {
      getAll: 'api/public/product',
      get: 'api/public/product/{id}',
    },
  },
};

export default api;
