import React, { useState } from 'react';

// Upewnij się, że ten plik fizycznie znajduje się w folderze src/pages/
import ProductManager from './pages/ProductManager'; 

// Tutaj wskazujemy na plik OrderPage.jsx, ale w kodzie używamy nazwy OrderTerminal
import OrderTerminal from './pages/OrderPage'; 

function App() {
    const [currentView, setCurrentView] = useState('products');

    return (
        <div className="container-fluid bg-light min-vh-100">
            {/* NAVBAR */}
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4 shadow-sm">
                <div className="container">
                    <a className="navbar-brand fw-bold" href="#">
                        🛍️ ShopSystem <span className="badge bg-secondary">Microservices</span>
                    </a>
                    
                    <div className="d-flex gap-2">
                        <button 
                            className={`btn ${currentView === 'products' ? 'btn-light' : 'btn-outline-light'}`}
                            onClick={() => setCurrentView('products')}
                        >
                            📦 Produkty
                        </button>
                        <button 
                            className={`btn ${currentView === 'orders' ? 'btn-light' : 'btn-outline-light'}`}
                            onClick={() => setCurrentView('orders')}
                        >
                            💳 Terminal Płatniczy
                        </button>
                    </div>
                </div>
            </nav>

            {/* CONTENT */}
            <div className="container pb-5">
                {currentView === 'products' ? <ProductManager /> : <OrderTerminal />}
            </div>

            {/* FOOTER */}
            <footer className="text-center text-muted mt-5 py-3 border-top">
                <small>Distributed Shop System &copy; 2026</small>
            </footer>
        </div>
    );
}

export default App;