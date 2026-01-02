
const API_BASE_ORDER = 'http://localhost:5295/api/orders';
const API_BASE_PRODUCT = 'http://localhost:5144/api/product';



export const createOrder = async (productId, quantity) => {
    
    const payload = {
        userId: 1, // user na sztywno
        items: [
            {
                productId: Number(productId), 
                quantity: Number(quantity)
            }
        ]
    };

    const response = await fetch(API_BASE_ORDER, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

   

    const data = await response.json();
    return { ...data, id: data.orderId, status: 0 };
};

export const payForOrder = async (orderId) => {
    const response = await fetch(`${API_BASE_ORDER}/${orderId}/pay`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    });
    return true; 
};

export const getOrder = async (orderId) => {
    const response = await fetch(`${API_BASE_ORDER}/${orderId}`);

    return await response.json();
};


export const getProducts = async () => {
    const response = await fetch(`${API_BASE_PRODUCT}/getProducts`);
    return await response.json();
};

export const createProduct = async (product) => {
    const response = await fetch(`${API_BASE_PRODUCT}/createProducts`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(product)
    });
    return await response.json();
};

export const deleteProduct = async (id) => {
    const response = await fetch(`${API_BASE_PRODUCT}/deleteProducts/${id}`, {
        method: 'DELETE'
    });
    return true;
};