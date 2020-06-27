const partsSearchApiUri = `${window.location.origin}/api/parts/search`

// Popup message for modal
function popupMessage(id) {
    var ele = document.getElementById(id);
    ele.classList.toggle("show");
}

// Parts Delete modal 
$('#deleteModal').on('show.bs.modal', function (e) {
    var modelData = $(e.relatedTarget).data('id');
    var form = $('#deleteOptionHidden');

    form.attr('value', modelData)
})

// Reusable modal trigger for ajax responses
function showModal(modalId, titleText, bodyText) {
    var modal = $(`#${modalId}`);
    modal.find(".modal-title").html(titleText);
    modal.find(".modal-body").html(bodyText);
    modal.modal("show");
}

// Search autocomplete for parts API
function partsAutoComplete(inputElementId, hiddenIdInputEle) {
    var input = document.getElementById(inputElementId);
    if (input != null) {
        input.addEventListener("input", updateListFromApi)
    }
    async function updateListFromApi() {
        closeAllLists();

        if (input.value == null || input.value.trim().length < 3) {
            return false;
        }
        // reset currentFocus on input
        var currentFocus = -1;
        // create container for items to be displayed
        var list = document.createElement("ul");
        list.setAttribute("class", "autocomplete-list");
        list.setAttribute("id", "autocomplete-list");
        this.parentNode.appendChild(list);

        // setup url with query strings
        var params = new URLSearchParams({
            partNumber: `${input.value}`
        });
        var uriComplete = `${partsSearchApiUri}?${params.toString()}`;

        // make api call and add inputs
        await fetch(uriComplete)
            .then(async response => await response.json())
            .then(json => {
                for (let i = 0; i < json.length; i++) {
                    // Create li and append it to the ul
                    var liEle = document.createElement("li");
                    liEle.setAttribute("class", "list-item")
                    list.appendChild(liEle);
                    // create spans and append them to the li
                    var partNumSpan = document.createElement("span");
                    partNumSpan.setAttribute("class", "list-item-header")
                    partNumSpan.innerHTML = `Part Number: ${json[i]["mfrsPartNumber"]} <br>`;
                    var descSpan = document.createElement("span");
                    descSpan.setAttribute("class", "list-item-desc")
                    descSpan.innerText = `${json[i]["name"]}`;
                    liEle.appendChild(partNumSpan);
                    liEle.appendChild(descSpan);
                    liEle.setAttribute("style", `z-index: ${i};`)

                    liEle.addEventListener("click", function (e) {
                        input.value = json[i]["mfrsPartNumber"];
                        document.getElementById(hiddenIdInputEle).value = json[i]["id"];
                        closeAllLists();
                        // the focus and blur is to trigger model validation after clicking the div
                        input.focus();
                        input.blur();
                    })
                }
            });
        // Add event listener for arrow keys
        input.addEventListener("keydown", function (e) {
            var list = document.getElementById("autocomplete-list");
            if (list) {
                listEles = list.getElementsByClassName("list-item");
                switch (e.key) {
                    case "ArrowUp":
                        currentFocus--;
                        setActive(listEles);
                        break;
                    case "ArrowDown": // DOWN
                        currentFocus++;
                        setActive(listEles);
                        break;
                    case "Enter": // ENTER, click element if applicable
                        e.preventDefault();
                        if (currentFocus > -1) {
                            if (listEles) {
                                $(".list-item-active").click()
                            }
                        }
                        break;
                }
            }
            function setActive(listEles) {
                if (!listEles) {
                    return;
                }
                // Remove active state before changing
                removeActive(listEles)
                if (currentFocus >= listEles.length) { currentFocus = 0 };
                if (currentFocus < 0) { currentFocus = listEles.length - 1 };
                listEles[currentFocus].classList.add("list-item-active");

            }
            function removeActive(listEles) {
                for (let i = 0; i < listEles.length; i++) {
                    listEles[i].classList.remove("list-item-active")
                }
            }
        });

    }
    function closeAllLists() {
        var lists = document.getElementsByClassName("autocomplete-list");
        for (let i = 0; i < lists.length; i++) {
            lists[i].parentNode.removeChild(lists[i]);
        };
    }

    // Close any lists if the document is clicked
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}

// Manual Ajax call for ModalPartsCreate partial due to IFormFile being submitted
$(function () {
    $("#newPartForm").submit(function (e) {
        e.preventDefault();
        // Prevent submit if field validation triggers
        if ($("#newPartForm > div.form-group").find("span.field-validation-error").length > 0) {
            return;
        };

        $.ajax({
            url: "/inventory/p",
            type: "post",
            data: new FormData(this),
            contentType: false,
            processData: false,
            success: function () {
                alert("Successfully saved new part.");
                location.reload();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert(xhr.responseText);
            }
        });
    });
});

// Modal trigger helper
function ToggleModalOnClick(btnId, modalId) {
    $(document).on("click", btnId, function(e) {
        e.preventDefault();
        $(modalId).modal("show");
    });
}
