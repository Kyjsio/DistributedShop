import React, { useState, useEffect } from 'react';
import { getProducts, createProduct, deleteProduct } from '../api/api';

function ProductManager() {
    const [products, setProducts] = useState([]);
    const [form, setForm] = useState({ name: '', description: '', price: 0, stockQuantity: 0 });

    useEffect(() => {
        loadProducts();
    }, []);

    const loadProducts = async () => {
        try {
            const data = await getProducts();
            setProducts(data);
        } catch (error) {
            console.error("Błąd �adowania produkt�w:", error);
        }
    };

    const handleAdd = async () => {
        try {
            await createProduct(form);
            setForm({ name: '', description: '', price: 0, stockQuantity: 0 }); 
            loadProducts();
        } catch (error) {
            alert("Błąd dodawania produktu");
        }
    };

    const handleDelete = async (id) => {
        if (!confirm("Czy na pewno chcesz usun�� ten produkt?")) return;
        try {
            await deleteProduct(id);
            loadProducts();
        } catch (error) {
            alert("Błąd usuwania");
        }
    };

    return (
        <div className="row">
            <div className="col-md-4">
                <div className="card shadow-sm mb-4">
                    <div className="card-header bg-success text-white">Dodaj Produkt</div>
                    <div className="card-body">
                        <div className="mb-2">
                            <label>Nazwa</label>
                            <input className="form-control" value={form.name} onChange={e => setForm({...form, name: e.target.value})} />
                        </div>
                        <div className="mb-2">
                            <label>Opis</label>
                            <input className="form-control" value={form.description} onChange={e => setForm({...form, description: e.target.value})} />
                        </div>
                        <div className="mb-2">
                            <label>Cena (PLN)</label>
                            <input type="number" className="form-control" value={form.price} onChange={e => setForm({...form, price: Number(e.target.value)})} />
                        </div>
                        <div className="mb-3">
                            <label>Ilość w magazynie</label>
                            <input type="number" className="form-control" value={form.stockQuantity} onChange={e => setForm({...form, stockQuantity: Number(e.target.value)})} />
                        </div>
                        <button onClick={handleAdd} className="btn btn-success w-100">Zapisz</button>
                    </div>
                </div>
            </div>

            <div className="col-md-8">
                <div className="card shadow-sm">
                    <div className="card-header">Lista Produkt�w</div>
                    <div className="card-body p-0">
                        <table className="table table-striped mb-0">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Nazwa</th>
                                    <th>Cena</th>
                                    <th>Magazyn</th>
                                    <th>Akcje</th>
                                </tr>
                            </thead>
                            <tbody>
                                {products.map(p => (
                                    <tr key={p.id}>
                                        <td>{p.id}</td>
                                        <td>{p.name}</td>
                                        <td>{p.price} PLN</td>
                                        <td>{p.stockQuantity} szt.</td>
                                        <td>
                                            <button onClick={() => handleDelete(p.id)} className="btn btn-danger btn-sm">Usu�</button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProductManager;