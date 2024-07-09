// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll('.btn-view-reply').forEach(button => {
    button.addEventListener('click', function () {
        const nextRow = this.nextElementSibling;
        nextRow.classList.toggle('hidden');
        if (nextRow.classList.contains('hidden')) {
            this.textContent = 'View Replies';
        } else {
            this.textContent = 'Hide Replies';
        }
    });
});

function togglePostButton() {
    var postInput = document.getElementById("postInput");
    var postTitleInput = document.getElementById("postTitleInput");
    var postButton = document.getElementById("postButton");
    var cancelButton = document.getElementById("cancelButton");

    if (postInput.value != "" && postTitleInput.value != "") {
        if (postButton.classList.contains('hidden')) {
            postButton.classList.toggle('hidden');
            cancelButton.classList.toggle('hidden');
        }
    } else if (postInput.value == "" || postTitleInput.value == ""){
        if (!postButton.classList.contains('hidden')) {
            postButton.classList.toggle('hidden');
            cancelButton.classList.toggle('hidden');
        }
    }
}
//document.getElementById("postInput").addEventListener("input", toggleCommentButton);

function showCommentButton(postId) {
    var commentInput = document.getElementById("commentInput(" + postId + ")");
    var cancelCommentButton = document.getElementById("cancelCommentButton(" + postId + ")");
    var commentButton = document.getElementById("commentButton(" + postId + ")");

    if (commentInput.value != "") {
        if (cancelCommentButton.classList.contains('hidden')) {
            commentButton.disabled = false;
        }
    } else {
        if (!cancelCommentButton.classList.contains('hidden')) {
            commentButton.disabled = true;
        }
    }
}

function hidePostButton() {
    var postButton = document.getElementById("postButton");
    var cancelButton = document.getElementById("cancelButton");
    postButton.classList.toggle('hidden');
    cancelButton.classList.toggle('hidden');
}

function showCommentBox(postId) {
    var commentBox = document.getElementById("commentBox(" + postId + ")");
    if (commentBox.classList.contains('hidden')) {
        commentBox.classList.toggle('hidden');
    }
}

function hideCommentBox(postId) {
    var commentBox = document.getElementById("commentBox(" + postId + ")");
    var cancelCommentButton = document.getElementById("cancelCommentButton(" + postId + ")");
    var commentButton = document.getElementById("commentButton(" + postId + ")");
    if (!commentBox.classList.contains('hidden')) {
        commentBox.classList.toggle('hidden');
        cancelCommentButton.classList.toggle('hidden');
        commentButton.classList.toggle('hidden');
    }
}

function upvotePost(postId) {
    var postUpvote = document.getElementById("postUpvote(" + postId + ")");
    var countUpvote = document.getElementById("countUpvote(" + postId + ")");

    if (!postUpvote.classList.contains('bg-green')) {
        countUpvote.innerText = Number(countUpvote.innerText) + 1;
        postUpvote.classList.toggle('bg-green');
        postUpvote.classList.toggle('bg-transparent');

    } else {
        countUpvote.innerText = Number(countUpvote.innerText) - 1;
        postUpvote.classList.toggle('bg-green');
        postUpvote.classList.toggle('bg-transparent');
    }
    $.ajax({
        type: 'POST',
        url: '@Url.Action("UpvotePost", "Module")',
        dataType: 'json',
        data: {
            postId: postId
        },
        success: function (response) {
            alert('succ');
        },
        error: function (xhr, status, error) {
            alert(xhr.responseText);
        }
    });
}

function upvoteComment(commentId) {
    var commentUpvote = document.getElementById("commentUpvote(" + commentId + ")");
    var commentCountUpvote = document.getElementById("commentCountUpvote(" + commentId + ")");

    if (!commentUpvote.classList.contains('bg-green')) {
        commentCountUpvote.innerText = Number(commentCountUpvote.innerText) + 1;
        commentUpvote.classList.toggle('bg-green');
        commentUpvote.classList.toggle('bg-transparent');

    } else {
        commentCountUpvote.innerText = Number(commentCountUpvote.innerText) - 1;
        commentUpvote.classList.toggle('bg-green');
        commentUpvote.classList.toggle('bg-transparent');
    }

    $.ajax({
        type: 'POST',
        url: '@Url.Action("UpvoteComment", "Module")',
        dataType: 'json',
        data: {
            commentId: commentId
        },
        success: function (response) {
        },
        error: function (xhr, status, error) {
        }
    });
}

function showTags() {
    var tagButton = document.getElementById("tagButton");
    if (tags.classList.contains('hidden')) {
        tags.classList.toggle('hidden');
        tagButton.innerHTML = "Hide Tags";

    } else {
        tags.classList.toggle('hidden');
        tagButton.innerHTML = "Add Tags";
    }
}

function showCreatePost() {
    var showCreatePostButton = document.getElementById("showCreatePostButton");
    var createPostDiv = document.getElementById("createPost");
    if (createPostDiv.classList.contains('hidden')) {
        createPostDiv.classList.toggle('hidden');
        showCreatePostButton.innerHTML = "- Hide Draft";

    } else {
        createPostDiv.classList.toggle('hidden');
        showCreatePostButton.innerHTML = "+ Create a Post";
    }
}


function toggleCommentEditButton(commentId) {
    var editButton = document.getElementById('editComment(' + commentId + ")");
    editButton.classList.toggle("hidden");
}

function toggleCancelEditCommentButton(commentId) {
    var cancelButton = document.getElementById('cancelEditComment(' + commentId + ")");
    cancelButton.classList.toggle("hidden");
}

function toggleDeleteCommentButton(commentId) {
    var deleteButton = document.getElementById('deleteComment(' + commentId + ")");
    deleteButton.classList.toggle("hidden");
}

function toggleSaveCommentChangesButton(commentId) {
    var saveButton = document.getElementById('saveChangesComment(' + commentId + ")");
    saveButton.classList.toggle("hidden");
}

function editComment(commentId) {
    var desc = document.getElementById('commentDescription(' + commentId + ")");
    var descInput = document.createElement('textarea');

    descInput.type = 'text';
    descInput.value = desc.innerText;
    desc.parentNode.replaceChild(descInput, desc);
    descInput.classList.toggle("w-100");
    descInput.classList.toggle("form-control");
    descInput.classList.toggle("col");
    descInput.id = "commentDescription(" + commentId + ")";

    toggleCommentEditButton(commentId);
    toggleCancelEditCommentButton(commentId);
    toggleDeleteCommentButton(commentId);
    toggleSaveCommentChangesButton(commentId);
}

function deleteComment(commentId) {
    toggleCancelEditCommentButton(commentId);
    toggleDeleteCommentButton(commentId);
    toggleSaveCommentChangesButton(commentId);

    $.ajax({
        type: 'POST',
        url: '@Url.Action("DeleteComment", "Module")',
        dataType: 'json',
        data: {
            commentId: commentId
        },
        success: function (response) {
            var content = document.getElementById("deleteCommentContent(" + commentId + ")");
            content.innerHTML = `
                        <div class="row">
                            <div class="col">
                                <p class="text-danger italic">Comment has been deleted...</p>
                            </div>
                        </div>
                    `;
        },
        error: function (xhr, status, error) {
        }
    });
}


function cancelEditComment(commentId) {
    $.ajax({
        type: 'POST',
        url: '@Url.Action("GetCommentDetails", "Module")',
        dataType: 'json',
        data: {
            commentId: commentId,
        },
        success: function (response) {
            var descriptionInput = document.getElementById('commentDescription(' + commentId + ")");
            var description = document.createElement('p');

            description.type = 'text';
            description.innerText = response.originalDescription;
            descriptionInput.parentNode.replaceChild(description, descriptionInput);
            description.id = "commentDescription(" + commentId + ")";

            toggleCommentEditButton(commentId);
            toggleCancelEditCommentButton(commentId);
            toggleDeleteCommentButton(commentId);
            toggleSaveCommentChangesButton(commentId);
        },
        error: function (xhr, status, error) {
        }
    });
}

function updateComment(commentId) {
    var descriptionInput = document.getElementById('commentDescription(' + commentId + ")");

    $.ajax({
        type: 'POST',
        url: '@Url.Action("EditComment", "Module")',
        dataType: 'json',
        data: {
            commentId: commentId,
            commentDescription: descriptionInput.value
        },
        success: function (response) {
            toggleCommentEditButton(commentId);
            toggleCancelEditCommentButton(commentId);
            toggleDeleteCommentButton(commentId);
            toggleSaveCommentChangesButton(commentId);

            var description = document.createElement('p');

            description.type = 'text';
            description.innerText = descriptionInput.value;
            descriptionInput.parentNode.replaceChild(description, descriptionInput);
            description.id = "commentDescription(" + commentId + ")";

            var commentEdited = document.getElementById('commentEdited(' + commentId + ")");
            if (commentEdited.classList.contains('hidden')) {
                commentEdited.classList.toggle('hidden');
            }
        },
        error: function (xhr, status, error) {
            alert('Do not leave inputs blank');
        }
    });
}






function togglePostEditButton(postId) {
    var editButton = document.getElementById('editPost(' + postId + ")");
    editButton.classList.toggle("hidden");
}

function togglePostCancelEditButton(postId) {
    var cancelButton = document.getElementById('cancelEditPost(' + postId + ")");
    cancelButton.classList.toggle("hidden");
}

function toggleDeletePostButton(postId) {
    var deleteButton = document.getElementById('deletePost(' + postId + ")");
    deleteButton.classList.toggle("hidden");
}

function toggleSavePostChangesButton(postId) {
    var saveButton = document.getElementById('saveChangesPost(' + postId + ")");
    saveButton.classList.toggle("hidden");
}

function editPost(postId) {
    var title = document.getElementById('postTitle(' + postId + ")");
    var titleInput = document.createElement('textarea');

    titleInput.type = 'text';
    titleInput.value = title.innerText;
    title.parentNode.replaceChild(titleInput, title);
    titleInput.classList.toggle("w-100");
    titleInput.classList.toggle("form-control");
    titleInput.id = "postTitle(" + postId + ")";

    var description = document.getElementById('postDescription(' + postId + ")");
    var descriptionInput = document.createElement('textarea');
    descriptionInput.type = 'text';
    descriptionInput.value = description.innerText;
    description.parentNode.replaceChild(descriptionInput, description);
    descriptionInput.classList.toggle("w-100");
    descriptionInput.classList.toggle("form-control");
    descriptionInput.id = "postDescription(" + postId + ")";

    togglePostEditButton(postId);
    togglePostCancelEditButton(postId);
    toggleSavePostChangesButton(postId);
    toggleDeletePostButton(postId);

}

function cancelEdit(postId) {
    $.ajax({
        type: 'POST',
        url: '@Url.Action("GetPostDetails", "Module")',
        dataType: 'json',
        data: {
            postId: postId,
        },
        success: function (response) {
            var titleInput = document.getElementById('postTitle(' + postId + ")");
            var title = document.createElement('h5');

            title.type = 'text';
            title.innerText = response.originalTitle;
            titleInput.parentNode.replaceChild(title, titleInput);
            title.id = "postTitle(" + postId + ")";

            var descriptionInput = document.getElementById('postDescription(' + postId + ")");
            var description = document.createElement('p');

            description.type = 'text';
            description.innerText = response.originalDescription;
            descriptionInput.parentNode.replaceChild(description, descriptionInput);
            description.id = "postDescription(" + postId + ")";

            togglePostEditButton(postId);
            togglePostCancelEditButton(postId);
            toggleSavePostChangesButton(postId);
            toggleDeletePostButton(postId);
        },
        error: function (xhr, status, error) {
        }
    });
}

function deletePost(postId) {
    togglePostCancelEditButton(postId);
    toggleSavePostChangesButton(postId);
    toggleDeletePostButton(postId);

    $.ajax({
        type: 'POST',
        url: '@Url.Action("DeletePost", "Module")',
        dataType: 'json',
        data: {
            postId: postId
        },
        success: function (response) {
            var content = document.getElementById("deletePostContent(" + postId + ")");
            content.innerHTML = `
                        <div class="row">
                            <div class="col">
                                <p class="text-danger italic">Post has been deleted...</p>
                            </div>
                        </div>
                    `;
        },
        error: function (xhr, status, error) {
        }
    });
}

function updatePost(postId, postTitle, postDescription) {
    var titleInput = document.getElementById('postTitle(' + postId + ")");
    var descriptionInput = document.getElementById('postDescription(' + postId + ")");

    $.ajax({
        type: 'POST',
        url: '@Url.Action("EditPost", "Module")',
        dataType: 'json',
        data: {
            postId: postId,
            postTitle: titleInput.value,
            postDescription: descriptionInput.value
        },
        success: function (response) {
            togglePostEditButton(postId);
            togglePostCancelEditButton(postId);
            toggleSavePostChangesButton(postId);
            toggleDeletePostButton(postId);

            var title = document.createElement('h5');

            title.type = 'text';
            title.innerText = titleInput.value;
            titleInput.parentNode.replaceChild(title, titleInput);
            title.id = "postTitle(" + postId + ")";

            var description = document.createElement('p');

            description.type = 'text';
            description.innerText = descriptionInput.value;
            descriptionInput.parentNode.replaceChild(description, descriptionInput);
            description.id = "postDescription(" + postId + ")";

            var postEdited = document.getElementById('postEdited(' + postId + ")");
            if (postEdited.classList.contains('hidden')) {
                postEdited.classList.toggle('hidden');
            }
        },
        error: function (xhr, status, error) {
            alert('Do not leave inputs blank');
        }
    });



}

