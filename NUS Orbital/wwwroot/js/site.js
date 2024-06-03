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
    var postButton = document.getElementById("postButton");
    var cancelButton = document.getElementById("cancelButton");
    if (postInput.value != "") {
        if (postButton.classList.contains('hidden')) {
            postButton.classList.toggle('hidden');
            cancelButton.classList.toggle('hidden');
        }
    } else {
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
            cancelCommentButton.classList.toggle('hidden');
            commentButton.classList.toggle('hidden');
        }
    } else {
        if (!cancelCommentButton.classList.contains('hidden')) {
            cancelCommentButton.classList.toggle('hidden');
            commentButton.classList.toggle('hidden');
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
        },
        error: function (xhr, status, error) {
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