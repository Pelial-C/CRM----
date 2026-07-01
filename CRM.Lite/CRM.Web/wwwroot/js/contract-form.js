(function () {
    const itemContainer = document.querySelector('[data-items]');
    const planContainer = document.querySelector('[data-plans]');
    const customerInput = document.querySelector('[data-contact-source]');
    const contactSelect = document.querySelector('[data-contact-target]');

    function renumberRows(container, rowSelector, prefix) {
        if (!container) return;
        container.querySelectorAll(rowSelector).forEach((row, index) => {
            row.querySelectorAll('[data-field]').forEach(input => {
                input.name = `${prefix}[${index}].${input.dataset.field}`;
            });
        });
    }

    function createItemRow(values) {
        const row = document.createElement('div');
        row.className = 'editable-row';
        row.setAttribute('data-item-row', '');
        row.innerHTML = `
            <input data-field="ProductName" value="${values?.productName || ''}" class="form-control" required placeholder="产品或服务名称" />
            <input data-field="Quantity" value="${values?.quantity || 1}" class="form-control" type="number" min="1" required />
            <input data-field="UnitPrice" value="${values?.unitPrice || 0}" class="form-control" type="number" min="0" step="0.01" required />
            <button type="button" class="btn btn-outline-danger" data-remove-row>删除</button>`;
        return row;
    }

    function createPlanRow(values) {
        const row = document.createElement('div');
        row.className = 'editable-row plan-row';
        row.setAttribute('data-plan-row', '');
        row.innerHTML = `
            <input data-field="PlanDate" value="${values?.planDate || new Date().toISOString().slice(0, 10)}" class="form-control" type="date" required />
            <input data-field="PlanAmount" value="${values?.planAmount || 0}" class="form-control" type="number" min="0.01" step="0.01" required />
            <input data-field="Description" value="${values?.description || ''}" class="form-control" placeholder="例如首付款、验收款" />
            <button type="button" class="btn btn-outline-danger" data-remove-row>删除</button>`;
        return row;
    }

    function addItem(values) {
        if (!itemContainer) return;
        itemContainer.appendChild(createItemRow(values));
        renumberRows(itemContainer, '[data-item-row]', 'Items');
    }

    function addPlan(values) {
        if (!planContainer) return;
        planContainer.appendChild(createPlanRow(values));
        renumberRows(planContainer, '[data-plan-row]', 'PaymentPlans');
    }

    document.querySelector('[data-add-item]')?.addEventListener('click', () => addItem());
    document.querySelector('[data-add-plan]')?.addEventListener('click', () => addPlan());

    document.addEventListener('click', event => {
        const button = event.target.closest('[data-remove-row]');
        if (!button) return;
        const row = button.closest('[data-item-row], [data-plan-row]');
        const container = row?.parentElement;
        row?.remove();
        if (container?.matches('[data-items]')) renumberRows(container, '[data-item-row]', 'Items');
        if (container?.matches('[data-plans]')) renumberRows(container, '[data-plan-row]', 'PaymentPlans');
    });

    document.getElementById('contractForm')?.addEventListener('submit', event => {
        if (itemContainer && itemContainer.querySelectorAll('[data-item-row]').length === 0) {
            event.preventDefault();
            alert('请至少添加一条合同明细。');
        }
    });

    async function loadContacts() {
        if (!customerInput || !contactSelect) return;
        const customerId = Number(customerInput.value);
        contactSelect.innerHTML = '<option value="">无联系人</option>';
        if (!customerId) return;

        const response = await fetch(`/Contract/GetContactsByCustomerId?customerId=${customerId}`);
        const result = await response.json();
        const current = contactSelect.dataset.currentContact || contactSelect.value;
        contactSelect.innerHTML = '<option value="">不指定联系人</option>';
        if (result.code === 200) {
            result.data.forEach(contact => {
                const option = document.createElement('option');
                option.value = contact.contactId;
                option.textContent = `${contact.contactName || '未命名'}${contact.phone ? ` · ${contact.phone}` : ''}`;
                if (String(contact.contactId) === String(current)) option.selected = true;
                contactSelect.appendChild(option);
            });
        }
    }

    customerInput?.addEventListener('change', loadContacts);
    loadContacts();

    if (itemContainer && itemContainer.querySelectorAll('[data-item-row]').length === 0) {
        addItem();
    } else {
        renumberRows(itemContainer, '[data-item-row]', 'Items');
    }
})();
