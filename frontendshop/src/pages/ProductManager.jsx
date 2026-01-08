import React, { useState, useEffect } from 'react';
import { getProducts, createProduct, deleteProduct, updateProduct } from '../api/api';

function ProductManager() {
    const [products, setProducts] = useState([]);
    const [editingId, setEditingId] = useState(null);
    const [form, setForm] = useState({ name: '', description: '', price: 0, stockQuantity: 0 });

    const [refreshKey, setRefreshKey] = useState(0);

    useEffect(() =>
    {
        let isMounted = true;

        const fetchData = async () => {
            try {
                const data = await getProducts();
                if (isMounted) {
                    setProducts(data);
                }
            } catch {
                console.error("Błąd ładowania produktów:");
            }
        };

        fetchData();

        return () => { isMounted = false; };
    }, [refreshKey]);

    const refreshList = () =>
    {
        setRefreshKey(prevKey => prevKey + 1);
    };

    const handleSave = async () =>
    {
        try
        {
            if (editingId)
            {
                await updateProduct(editingId, form);
                alert("Zaktualizowano produkt!");
            }
            else
            {
                await createProduct(form);
                alert("Dodano produkt!");
            }

            setForm({ name: '', description: '', price: 0, stockQuantity: 0 });
            setEditingId(null);

            refreshList();
        }
        catch
        {
            alert("Błąd zapisu");
        }
    };

    const handleDelete = async (id) =>
    {
        if (!window.confirm("Czy na pewno chcesz usunąć ten produkt?")) return;
        try
        {
            await deleteProduct(id);
            refreshList();
        }
        catch
        {
            alert("Błąd usuwania");
        }
    };

    const startEdit = (product) => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
        setForm({
            name: product.name,
            description: product.description,
            price: product.price,
            stockQuantity: product.stockQuantity
        });
        setEditingId(product.id);
    };
    return (
        <div className="row">
            <div className="col-md-4">
                {/*Dodaj/Edycja*/}
                <div className={`card shadow-sm mb-4 ${editingId ? 'border-warning' : 'border-success'}`}>
                    <div className={`card-header text-white ${editingId ? 'bg-warning' : 'bg-success'}`}>
                        {editingId ? 'Edytuj Produkt' : 'Dodaj Produkt'}
                    </div>
                    <div className="card-body">
                        <div className="mb-2">
                            <label>Nazwa</label>
                            <input className="form-control" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
                        </div>
                        <div className="mb-2">
                            <label>Opis</label>
                            <input className="form-control" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
                        </div>
                        <div className="mb-2">
                            <label>Cena (PLN)</label>
                            <input type="number" className="form-control" value={form.price} onChange={e => setForm({ ...form, price: Number(e.target.value) })} />
                        </div>
                        <div className="mb-3">
                            <label>Ilość w magazynie</label>
                            <input type="number" className="form-control" value={form.stockQuantity} onChange={e => setForm({ ...form, stockQuantity: Number(e.target.value) })} />
                        </div>

                        <button onClick={handleSave} className={`btn w-100 ${editingId ? 'btn-warning' : 'btn-success'}`}>
                            {editingId ? 'Zaktualizuj' : 'Zapisz'}
                        </button>

                        {editingId && (
                            <button onClick={() => { setEditingId(null); setForm({ name: '', description: '', price: 0, stockQuantity: 0 }); }} className="btn btn-secondary w-100 mt-2">
                                Anuluj
                            </button>
                        )}
                    </div>
                </div>
            </div>
            {/* Lista produktów */}
            <div className="col-md-8">
                <div className="card shadow-sm">
                    <div className="card-header">Lista Produktów</div>
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
                                            <button onClick={() => startEdit(p)} className="btn btn-primary btn-sm me-2">Edytuj</button>
                                            <button onClick={() => handleDelete(p.id)} className="btn btn-danger btn-sm">Usuń</button>
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