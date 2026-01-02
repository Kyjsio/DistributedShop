import React, { useState, useEffect } from 'react';
import { createOrder, payForOrder, getOrder, getProducts } from '../api/api';

const OrderTerminal = () => {
    const [products, setProducts] = useState([]);         // Lista produktów z API
    const [selectedProductId, setSelectedProductId] = useState(''); // Wybrane ID
    const [quantity, setQuantity] = useState(1);          // Wybrana ilość
    
    const [order, setOrder] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [logs, setLogs] = useState([]);

    useEffect(() => {
        loadProductsFromApi();
    }, []);

    const loadProductsFromApi = async () => {
        try {
            const data = await getProducts();
            setProducts(data);
            if (data.length > 0) {
                setSelectedProductId(data[0].id);
            }
        } catch (error) {
            addLog(`⚠️ Nie można pobrać produktów: ${error.message}`);
        }
    };

    const addLog = (message) => {
        const time = new Date().toLocaleTimeString();
        setLogs(prev => [`[${time}] ${message}`, ...prev]);
    };

    const handleCreateOrder = async () => {
        if (!selectedProductId) {
            alert("Wybierz produkt z listy!");
            return;
        }

        setIsLoading(true);
        try {
            const data = await createOrder(selectedProductId, quantity);
            
            setOrder(data);
            
            const productInfo = products.find(p => p.id == selectedProductId);
            const productName = productInfo ? productInfo.name : `ID: ${selectedProductId}`;
            
            addLog(`✅ Utworzono zamówienie na: ${productName} (${quantity} szt.). Cena wg serwera: ${data.totalAmount} EUR`);
        } catch (error) {
            addLog(`❌ Błąd: ${error.message}`);
        } finally {
            setIsLoading(false);
        }
    };

    const handlePayOrder = async () => {
        if (!order) return;
        setIsLoading(true);
        try {
            addLog('📡 Płatność w toku (RabbitMQ)...');
            await payForOrder(order.id);
            addLog('📨 Wysłano żądanie płatności.');
        } catch (error) {
            addLog('❌ Błąd płatności.');
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        let interval = null;
        if (order && order.status !== 1 && order.status !== 2) { 
            interval = setInterval(async () => {
                try {
                    const updatedOrder = await getOrder(order.id);
                    if (updatedOrder.status !== order.status) {
                        setOrder(updatedOrder);
                        if (updatedOrder.status === 1) addLog('💰 SUKCES! Status: PAID.');
                        if (updatedOrder.status === 2) addLog('⛔ ANULOWANO! Status: CANCELLED.');
                    }
                } catch (err) { console.error(err); }
            }, 2000);
        }
        return () => { if (interval) clearInterval(interval); };
    }, [order]);

    const getStatusBadge = (status) => {
        if (status === 1) return <span className="badge bg-success">PAID</span>;
        if (status === 2) return <span className="badge bg-danger">CANCELLED</span>;
        return <span className="badge bg-warning text-dark">CREATED</span>;
    };

    return (
        <div className="container mt-4" style={{ maxWidth: '800px' }}>
            <div className="card shadow-sm">
                <div className="card-header bg-dark text-white d-flex justify-content-between align-items-center">
                    <h5 className="mb-0">Symulator Zakupów</h5>
                    <button onClick={loadProductsFromApi} className="btn btn-sm btn-outline-light">🔄 Odśwież produkty</button>
                </div>
                <div className="card-body">
                    
                    <div className="p-4 mb-4 bg-light border rounded">
                        <h6 className="text-muted text-uppercase mb-3">Wybierz towar z magazynu (ProductService)</h6>
                        
                        <div className="row g-3 align-items-end">
                            <div className="col-md-7">
                                <label className="form-label">Produkt</label>
                                <select 
                                    className="form-select" 
                                    value={selectedProductId} 
                                    onChange={(e) => setSelectedProductId(e.target.value)}
                                    disabled={!!order}
                                >
                                    {products.length === 0 && <option>Ładowanie produktów...</option>}
                                    {products.map(p => (
                                        <option key={p.id} value={p.id}>
                                            {p.name} — {p.price} PLN (Stan: {p.stockQuantity})
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="col-md-2">
                                <label className="form-label">Ilość</label>
                                <input 
                                    type="number" 
                                    className="form-control" 
                                    min="1" 
                                    value={quantity} 
                                    onChange={(e) => setQuantity(e.target.value)}
                                    disabled={!!order}
                                />
                            </div>

                            <div className="col-md-3">
                                <button 
                                    onClick={handleCreateOrder} 
                                    className="btn btn-primary w-100"
                                    disabled={!!order || isLoading || products.length === 0}
                                >
                                    Kup Teraz
                                </button>
                            </div>
                        </div>
                    </div>

                    <hr />

                    {order && (
                        <div className="alert alert-secondary text-center">
                            <h4>Zamówienie #{order.id}</h4>
                            <div className="fs-5 mb-2">Do zapłaty: <strong>{order.totalAmount} EUR</strong></div>
                            <div className="mb-3">{getStatusBadge(order.status)}</div>

                            {order.status === 0 && (
                                <button 
                                    onClick={handlePayOrder} 
                                    disabled={isLoading}
                                    className="btn btn-warning w-100 fw-bold btn-lg"
                                >
                                    {isLoading ? 'Przetwarzanie...' : '💳 Zapłać'}
                                </button>
                            )}
                            
                            {order.status !== 0 && (
                                <button onClick={() => { setOrder(null); setLogs([]); }} className="btn btn-outline-secondary mt-3 btn-sm">
                                    Nowe zamówienie
                                </button>
                            )}
                        </div>
                    )}

                    <div className="bg-dark text-light p-3 rounded mt-3" style={{ height: '200px', overflowY: 'auto', fontFamily: 'monospace', fontSize: '0.85rem' }}>
                        <div className="text-muted border-bottom border-secondary mb-2 pb-1">System Logs:</div>
                        {logs.map((log, index) => <div key={index}>{log}</div>)}
                    </div>

                </div>
            </div>
        </div>
    );
};

export default OrderTerminal;