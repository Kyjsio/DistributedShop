import React, { useState } from 'react';

import ProductManager from './pages/ProductManager'; 

import OrderTerminal from './pages/OrderPage'; 

function App() {
    const [currentView, setCurrentView] = useState('products');

    return (
        <div className="container-fluid bg-light min-vh-100">

            <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4 shadow-sm">
                <div className="container">
                    <a className="navbar-brand fw-bold" href="#">
                        ShopSystem
                    </a>
                    
                    <div className="d-flex gap-2">
                        <button 
                            className={`btn ${currentView === 'products' ? 'btn-light' : 'btn-outline-light'}`}
                            onClick={() => setCurrentView('products')}
                        >
                            Produkty
                        </button>
                        <button 
                            className={`btn ${currentView === 'orders' ? 'btn-light' : 'btn-outline-light'}`}
                            onClick={() => setCurrentView('orders')}
                        >
                            Zamowienie
                        </button>
                    </div>
                </div>
            </nav>


            <div className="container pb-5">
                {currentView === 'products' ? <ProductManager /> : <OrderTerminal />}
            </div>


            <footer className="text-center text-muted mt-5 py-3 border-top">
                <small>Distributed Shop System &copy; 2026</small>
            </footer>
        </div>
    );
}

export default App;